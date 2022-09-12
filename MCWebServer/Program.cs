using MCWebServer.Config;
using MCWebServer.Log;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
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
            MCWebServer.Hamachi.NetworkingTools.CheckNetworking();

            // Invoke importing the config
            _ = Config.Instance.ToString();

            // Start Hamachi
            if(args.Contains("--start-hamachi"))
                _ = MCWebServer.Hamachi.HamachiClient.LogOn();

            // Start Webserver
            if(args.Contains("--web-server"))
                CreateHostBuilder(args).Build().Start();

            //Start Discord bot
            if(args.Contains("--discord-bot"))
                await MCWebServer.Discord.DiscordBot.Initialize();

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
