global using System;
global using System.Linq;

using Application.Config;
using HamachiHelper;
using Loggers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using System.IO;
using System.Threading.Tasks;

namespace Web_Test
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
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
                config.MinecraftServerInitRamMB, 5);
            

            // Start Hamachi
            if(args.Contains("--start-hamachi"))
                _ = HamachiClient.LogOn();

            // Start Webserver
            if(args.Contains("--web-server"))
                CreateHostBuilder(args).Build().Start();

            //Start Discord bot
            if(args.Contains("--discord-bot"))
                await DiscordBot.Discord.DiscordBot.Initialize(config.DiscordBotToken);

            await Task.Delay(-1);
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls($"https://*:{Config.Instance.WebServerPortHttps}", $"http://*:{Config.Instance.WebServerPortHttp}");
    }
}
