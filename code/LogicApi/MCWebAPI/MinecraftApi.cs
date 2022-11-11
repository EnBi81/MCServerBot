using Application.DAOs;
using Application.Minecraft;
using Application.Permissions;
using DataStorageSQLite.Implementation;
using Loggers;
using Loggers.Loggers;
using MCWebAPI.Auth;
using MCWebAPI.Middlewares;
using MCWebAPI.Utils;
using MCWebAPI.WebSocketHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.DTOs;
using Shared.Model;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace MCWebAPI
{
    /// <summary>
    /// Minecraft web api instance
    /// </summary>
    public class MinecraftApi
    {
        /// <summary>
        /// Runs the web api and blocks the thread.
        /// </summary>
        /// <param name="args">arguments</param>
        public void Run(string[] args)
        {
            // setting resources folder in environment variables
            Environment.SetEnvironmentVariable("RESOURCES_FOLDER", "Resources");


            var builder = WebApplication.CreateBuilder(args);
            
            
            var logger = LogService.GetService<WebApiLogger>();
            logger.Log("start", "Starting web api in " + Environment.CurrentDirectory);

            
            
            // adding appsettings.json (because I moved it to the Properties folder)
            builder.Configuration.AddJsonFile("Properties\\appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();

            //https://www.youtube.com/watch?v=SmItxjIUiLc dubdomain routing

            #region Register Services

            IServiceCollection serviceCollection = builder.Services;
            

            serviceCollection.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            serviceCollection.AddEndpointsApiExplorer();
            
            serviceCollection.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = false;
                setup.ReportApiVersions = false;
            });
            
            serviceCollection.AddSwaggerGen();
            serviceCollection.ConfigureOptions<ConfigureSwaggerOptions>();

            serviceCollection.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
                setup.AssumeDefaultVersionWhenUnspecified = false;
            });
            
            
            // Configs
            serviceCollection.AddSingleton(new MinecraftConfig
            {
                MaxSumOfDiskSpaceGB = int.Parse(builder.Configuration["MinecraftSettings:MaxDiskSpaceGB"]),
                MinecraftServerInitRamMB = int.Parse(builder.Configuration["MinecraftSettings:ServerInitRamMB"]),
                MinecraftServerMaxRamMB = int.Parse(builder.Configuration["MinecraftSettings:ServerMaxRamMB"]),
                JavaLocation = builder.Configuration["Paths:JavaLocation"],
                MinecraftServersBaseFolder = builder.Configuration["Paths:MinecraftServersBaseFolder"],
                MinecraftServerHandlerPath = builder.Configuration["Paths:MinecraftServerHandler"],
            });

            // Loggers
            serviceCollection.AddSingleton(LogService.GetService<MinecraftLogger>());
            serviceCollection.AddSingleton(LogService.GetService<WebApiLogger>());

            //Model
            serviceCollection.AddSingletonAndInit<IDatabaseAccess, DataStorageSQLiteImpl>
                (async databaseAccess => await databaseAccess.DatabaseSetup.Setup(builder.Configuration["ConnectionStrings:SQLite"]));
            serviceCollection.AddSingletonAndInit<IServerPark, ServerPark>(async serverPark => await serverPark.InitializeAsync());
            serviceCollection.AddSingleton<IPermissionLogic, PermissionLogic>();


            // API
            serviceCollection.AddSingleton<SocketPool>();


            // API Auth
            serviceCollection.AddScoped<IAuthService, AuthService>();
            serviceCollection.AddAuthorizationCore(options =>
            {
                options.AddPolicy("DiscordBot", a =>
                    a.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, DataUserType.Discord.ToString()));
            });
            serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });


            #endregion

            logger.Log("start", "Building application");

            var app = builder.Build();

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
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            app.UseCors(cors => cors
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials());


            app.UseMiddleware<ApiLoggerMiddleware>();
            app.UseMiddleware<ExceptionHandlerMiddleware>();


            logger.Log("start", "Running application");
            
            app.Run();
        }
    }
}
