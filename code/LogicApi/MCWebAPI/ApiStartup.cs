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
        /// <param name="config">config</param>
        public void Run(string[] args, Config config)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Register Services

            IServiceCollection serviceCollection = builder.Services;

            serviceCollection.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            serviceCollection.AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
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
                        new string[] {}
                    }
                });
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            })




            // Configs
            .AddSingleton(new MinecraftConfig
            {
                JavaLocation = config.JavaLocation,
                MaxSumOfDiskSpaceGB = config.MinecraftMaxDiskSpaceGB,
                MinecraftServerHandlerPath = config.MinecraftServerHandlerPath,
                MinecraftServerInitRamMB = config.MinecraftServerInitRamMB,
                MinecraftServerMaxRamMB = config.MinecraftServerMaxRamMB,
                MinecraftServersBaseFolder = config.MinecraftServersBaseFolder,
            })

            // Loggers
            .AddSingleton(LogService.GetService<MinecraftLogger>())
            .AddSingleton(LogService.GetService<WebApiLogger>())

            //Model
            .AddSingletonAndInit<IDatabaseAccess, DataStorageSQLiteImpl>
                (async databaseAccess => await databaseAccess.DatabaseSetup.Setup(builder.Configuration["ConnectionStrings:SQLite"]))
            .AddSingletonAndInit<IServerPark, ServerPark>(async serverPark => await serverPark.InitializeAsync())
            .AddSingleton<IPermissionLogic, PermissionLogic>()


            // API
            .AddSingleton<SocketPool>()


            // API Auth
            .AddScoped<IAuthService, AuthService>()
            .AddAuthorizationCore(options =>
            {
                options.AddPolicy("DiscordBot", a =>
                    a.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, DataUserType.Discord.ToString()));
            })
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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

            app.UseMiddleware<MCApiLoggerMiddleware>();
            app.UseMiddleware<MCExceptionHandlerMiddleware>();

            app.Run();
        }
    }
}