using HamachiHelper;
using Loggers;
using MCWebApp;
using MCWebApp.Config;

//https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-6.0
//https://learn.microsoft.com/en-us/aspnet/core/security/authorization/simple?source=recommendations&view=aspnetcore-6.0

LogService logService = new LogService()
                .SetupLogger<DiscordLogger>()
                .SetupLogger<HamachiLogger>()
                .SetupLogger<MinecraftLogger>()
                .SetupLogger<WebLogger>()
                .SetupLogger<ConfigLogger>()
                .SetupLogger<NetworkLogger>();

LogService.RegisterLogService(logService);

//check internet connection
NetworkingTools.CheckNetworking();

// Invoke importing the config
var config = Config.Instance;

HamachiClient.Setup(config.HamachiLocation);


Application.MinecraftConfig.SetupConfig(
    config.MinecraftServersBaseFolder, config.JavaLocation,
    config.MinecraftServerHandlerPath, config.MinecraftServerMaxRamMB,
    config.MinecraftServerInitRamMB, config.MinecraftMaxDiskSpaceGB);

DiscordBot.DiscordConfig.SetupConfig(config.DiscordBotToken, config.WebServerPortHttps);


// Start Hamachi
if (args.Contains("--start-hamachi"))
    _ = HamachiClient.LogOn();

// Start Webserver
if (args.Contains("--web-server"))
    await MCWebServer.StartWebServer(args);

//Start Discord bot
if (args.Contains("--discord-bot"))
    await DiscordBot.Bot.DiscordBot.Initialize(config.DiscordBotToken);

await Task.Delay(-1);

