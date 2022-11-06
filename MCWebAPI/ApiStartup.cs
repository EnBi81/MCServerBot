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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.DTOs;
using Shared.Model;
using Swashbuckle.AspNetCore.Filters;
using System.Management;
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
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme (JWT). Example: \"bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            });




            // Configs
            serviceCollection.AddSingleton(new MinecraftConfig
            {
                JavaLocation = config.JavaLocation,
                MaxSumOfDiskSpaceGB = config.MinecraftMaxDiskSpaceGB,
                MinecraftServerHandlerPath = config.MinecraftServerHandlerPath,
                MinecraftServerInitRamMB = config.MinecraftServerInitRamMB,
                MinecraftServerMaxRamMB = config.MinecraftServerMaxRamMB,
                MinecraftServersBaseFolder = config.MinecraftServersBaseFolder
            });

            // Loggers
            serviceCollection.AddSingleton(LogService.GetService<MinecraftLogger>());
            serviceCollection.AddSingleton(LogService.GetService<WebApiLogger>());

            //Model
            serviceCollection.AddSingletonAndInit<IDatabaseAccess, DataStorageSQLiteImpl>
                (async databaseAccess => await databaseAccess.DatabaseSetup.Setup("Data Source=C:\\Users\\enbi8\\source\\repos\\MCServerBot\\MCWebApp\\Resources\\eventdata.db;Version=3;"));
            serviceCollection.AddSingletonAndInit<IServerPark, ServerPark>(async serverPark => await serverPark.InitializeAsync());
            serviceCollection.AddSingleton<IPermissionLogic, PermissionLogic>();


            // API
            serviceCollection.AddSingleton<SocketPool>();


            // API Auth
            serviceCollection.AddScoped<IAuthService, AuthService>();
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
            serviceCollection.AddAuthorizationCore(options =>
            {
                options.AddPolicy("DiscordBot", a =>
                    a.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, DataUserType.Discord.ToString()));
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

            

            app.MapControllers();

            app.UseAuthentication();
            app.UseAuthorization();

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
