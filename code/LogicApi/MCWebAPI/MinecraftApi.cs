using Loggers;
using Loggers.Loggers;
using MCWebAPI.Middlewares;
using MCWebAPI.Utils.Setup;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SharedPublic.Model;
using SignalRUtils;
using System.Reflection;

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
            // setting resources folder in environment variables (important for loggers, and also minecraft)
            Environment.SetEnvironmentVariable("RESOURCES_FOLDER", "Resources");

            logger = LogService.GetService<WebApiLogger>();

            logger.Log("info", "Calling: " + Assembly.GetCallingAssembly()?.Location);
            logger.Log("info", "Executing: " + Assembly.GetExecutingAssembly()?.Location);
            logger.Log("info", "Entry: " + Assembly.GetEntryAssembly()?.Location);

            // create the builder
            builder = WebApplication.CreateBuilder(args);
            
            // get the logger
            
            logger.Log("start", "Starting web api in " + Environment.CurrentDirectory);

            
            // adding appsettings.json (because I moved it to the Properties folder)
            builder.Configuration.AddJsonFile("Properties\\appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
            
            // important part: add the services
            builder.Services.AddBaseApiServices()
                .AddModelElements(builder.Configuration)
                .AddAPIElements(builder.Configuration);

            builder.Logging.ClearProviders();
            
            builder.WebHost.UseUrls(
                $"https://*:{builder.Configuration["ApiSettings:HttpsPort"]}", 
                $"http://*:{builder.Configuration["ApiSettings:HttpPort"]}");

            logger.Log("start", "Listening on ports: https://*:" + builder.Configuration["ApiSettings:HttpsPort"]);

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
                    
                    options.InjectJavascript("/swagger-extension.js");
                });
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<ApiLoggerMiddleware>();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            

            app.UseCors(cors => cors
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials());

            app.MapHubs();



            _logger.Log("start", "Running application");

            await app.StartAsync();


            // check if swagger is running this program, if yes, then dont initialize the IServerPark
            if(!Assembly.GetEntryAssembly()?.Location?.EndsWith("swagger.dll") ?? true)
                app.Services.GetRequiredService<IServerPark>();
        }
    }
    
}
