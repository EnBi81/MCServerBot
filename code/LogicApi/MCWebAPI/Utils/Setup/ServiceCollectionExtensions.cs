using Application.DAOs;
using Application.Minecraft;
using Application.Permissions;
using DataStorageSQLite.Implementation;
using Loggers;
using Loggers.Loggers;
using MCWebAPI.Auth;
using MCWebAPI.WebSocketHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharedPublic.DTOs;
using SharedPublic.Model;
using System.Text;
using System.Security.Claims;
using Application.Minecraft.Configs;

namespace MCWebAPI.Utils.Setup;

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

        collection.AddSignalR();

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
            MaxSumOfDiskSpaceGB = int.Parse(configuration["MinecraftSettings:ServerPark:MaxDiskSpaceGB"]!),
            JavaLocation = configuration["Paths:JavaLocation"]!,
            MinecraftServersBaseFolder = configuration["Paths:MinecraftServersBaseFolder"]!,
            MinecraftServerHandlerPath = configuration["Paths:MinecraftServerHandler"]!,
            BackupFolder = configuration["Paths:BackupFolder"]!,
            ServerConfig = new MinecraftServerConfig
            {
                ServerInitRamMB = int.Parse(configuration["MinecraftSettings:ServerProcess:ServerInitRamMB"]!),
                ServerMaxRamMB = int.Parse(configuration["MinecraftSettings:ServerProcess:ServerMaxRamMB"]!),
                AutoBackupAfterUptimeMinute = int.Parse(configuration["MinecraftSettings:Backup:AutoBackupAfterUptimeMinute"]!),
                MaxAutoBackup = int.Parse(configuration["MinecraftSettings:Backup:MaxAutoBackup"]!),
                MaxManualBackup = int.Parse(configuration["MinecraftSettings:Backup:MaxManualBackup"]!),
            }
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