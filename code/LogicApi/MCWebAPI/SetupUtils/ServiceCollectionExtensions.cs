﻿using Application.DAOs;
using Application.Minecraft;
using Application.Permissions;
using DataStorageSQLite.Implementation;
using Loggers.Loggers;
using Loggers;
using Microsoft.Extensions.DependencyInjection;
using Shared.Model;
using MCWebAPI.Utils;
using MCWebAPI.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MCWebAPI.WebSocketHandler;
using System.Security.Claims;
using Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.SetupUtils
{
    /// <summary>
    /// Organizes the services that are added to the IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the basic api services required for the app.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection AddBaseApiServices(this IServiceCollection collection)
        {
            collection.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            collection.AddEndpointsApiExplorer();

            collection.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = false;
                setup.ReportApiVersions = false;
            });

            collection.AddSwaggerGen();
            collection.ConfigureOptions<ConfigureSwaggerOptions>();

            collection.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
                setup.AssumeDefaultVersionWhenUnspecified = false;
            });

            return collection;
        }

        /// <summary>
        /// Adds the custom model interfaces and implementations to the IServiceCollection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddModelElements(this IServiceCollection collection, IConfiguration configuration)
        {
            // Configs
            collection.AddSingleton(new MinecraftConfig
            {
                MaxSumOfDiskSpaceGB = int.Parse(configuration["MinecraftSettings:MaxDiskSpaceGB"]),
                MinecraftServerInitRamMB = int.Parse(configuration["MinecraftSettings:ServerInitRamMB"]),
                MinecraftServerMaxRamMB = int.Parse(configuration["MinecraftSettings:ServerMaxRamMB"]),
                JavaLocation = configuration["Paths:JavaLocation"],
                MinecraftServersBaseFolder = configuration["Paths:MinecraftServersBaseFolder"],
                MinecraftServerHandlerPath = configuration["Paths:MinecraftServerHandler"],
            });

            // Loggers
            collection.AddSingleton(LogService.GetService<MinecraftLogger>());
            collection.AddSingleton(LogService.GetService<WebApiLogger>());

            //Model
            collection.AddSingletonAndInit<IDatabaseAccess, DataStorageSQLiteImpl>
                (async databaseAccess => await databaseAccess.DatabaseSetup.Setup(configuration["ConnectionStrings:SQLite"]));
            collection.AddSingletonAndInit<IServerPark, ServerPark>(async serverPark => await serverPark.InitializeAsync());
            collection.AddSingleton<IPermissionLogic, PermissionLogic>();

            return collection;
        }

        /// <summary>
        /// Adds the api elements to the service collection, such as Socketpool and authentication.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAPIElements(this IServiceCollection collection, IConfiguration configuration)
        {
            // API
            collection.AddSingleton<SocketPool>();


            // API Auth
            collection.AddScoped<IAuthService, AuthService>();
            collection.AddAuthorizationCore(options =>
            {
                options.AddPolicy("DiscordBot", a =>
                    a.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, DataUserType.Discord.ToString()));
            });
            collection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidIssuer = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });

            return collection;
        }
    }
}