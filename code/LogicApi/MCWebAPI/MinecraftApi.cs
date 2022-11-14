using Loggers;
using Loggers.Loggers;
using MCWebAPI.Middlewares;
using MCWebAPI.Utils.Setup;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Shared.Model;

namespace MCWebAPI
{
    /// <summary>
    /// Minecraft web api instance
    /// </summary>
    public class MinecraftApi
    {
        private readonly WebApplication _app;
        private readonly WebApiLogger _logger;

        public MinecraftApi(string[] args)
        {
            GetAppBuilder(args, out var builder, out _logger);
            _app = builder.Build();
        }

        
        private static void GetAppBuilder(string[] args, out WebApplicationBuilder builder, out WebApiLogger logger)
        {
            // setting resources folder in environment variables
            Environment.SetEnvironmentVariable("RESOURCES_FOLDER", "Resources");

            // create the builder
            builder = WebApplication.CreateBuilder(args);
            
            // get the logger
            logger = LogService.GetService<WebApiLogger>();
            logger.Log("start", "Starting web api in " + Environment.CurrentDirectory);

            
            // adding appsettings.json (because I moved it to the Properties folder)
            builder.Configuration.AddJsonFile("Properties\\appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
            
            // important part: add the services
            builder.Services.AddBaseApiServices()
                .AddModelElements(builder.Configuration)
                .AddAPIElements(builder.Configuration);

            
            logger.Log("start", "Building application");
        }



        public async Task Stop()
        {
            var serverPark = _app.Services.GetRequiredService<IServerPark>();

            if (serverPark.ActiveServer?.IsRunning is true)
                await serverPark.StopActiveServer();
            
            await _app.StopAsync();
        }

        /// <summary>
        /// Starts the web api asynchronously
        /// </summary>
        public async Task StartAsync()
        {
            var app = _app;

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }

                    options.DisplayRequestDuration();
                    options.DisplayOperationId();
                    options.EnableTryItOutByDefault();
                    options.OAuthScopes("bearer");

                    options.InjectStylesheet("/swagger-hubs.css");
                    options.InjectJavascript("https://code.jquery.com/jquery-3.6.1.min.js");
                    options.InjectJavascript("https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/6.0.1/signalr.js");
                    options.InjectJavascript("/swagger-extension.js");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.MapHubs();
            app.UseRouting();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapControllers();

            app.UseEndpoints(endpoint => endpoint.MapGet("swaggergethubs", () => { return new List<string> { "ServerParkHub" }; }));

            app.UseCors(cors => cors
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials());


            app.UseMiddleware<ApiLoggerMiddleware>();
            app.UseMiddleware<ExceptionHandlerMiddleware>();


            _logger.Log("start", "Running application");

            await app.StartAsync();

            app.Services.GetRequiredService<IServerPark>();
        }
    }
}
