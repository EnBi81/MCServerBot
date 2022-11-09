﻿using Application.DAOs;
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
            var logger = LogService.GetService<WebApiLogger>();


            logger.Log("start", "Starting web api in " + Environment.CurrentDirectory);


            var builder = WebApplication.CreateBuilder(args);

            // adding appsettings.json (because I moved it to the Properties folder)
            builder.Configuration.AddJsonFile("Properties\\appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();

            #region Register Services

            IServiceCollection serviceCollection = builder.Services;

            serviceCollection.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Standard Authorization header using the Bearer scheme (JWT). Example: \"bearer {token}\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
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
                app.UseSwaggerUI();
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

            app.UseMiddleware<MCExceptionHandlerMiddleware>();
            app.UseMiddleware<MCApiLoggerMiddleware>();

            logger.Log("start", "Running application");
            
            app.Run();
        }
    }
}
