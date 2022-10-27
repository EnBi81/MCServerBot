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
bool internetConnected = NetworkingTools.CheckNetworking();

// Invoke importing the config
var config = Config.Instance;

HamachiClient.Setup(config.HamachiLocation);

await DataStorage.DatabaseAccess.SQLite.Setup("Data Source=C:\\Users\\enbi8\\source\\repos\\MCServerBot\\MCWebApp\\Resources\\eventdata.db;Version=3;");


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
    await MCWebServer.StartWebServer(args, config.WebServerPortHttps, config.WebServerPortHttp);

//Start Discord bot
if (args.Contains("--discord-bot"))
{
    if (internetConnected)
        await DiscordBot.Bot.DiscordBot.Initialize(config.DiscordBotToken);
    else
        LogService.GetService<DiscordLogger>().LogError("Cannot start discord bot: NO INTERNET CONNECTION");

}
    

await Task.Delay(-1);

