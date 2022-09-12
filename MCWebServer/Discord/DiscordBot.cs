using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MCWebServer.Discord.Services;
using MCWebServer.MinecraftServer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MCWebServer.PermissionControll;
using MCWebServer.Log;

namespace MCWebServer.Discord
{
    internal class DiscordBot
    {
        public static DiscordBot Bot { get; private set; }
        public static async Task Initialize()
        {
            Bot = new DiscordBot();
            await Bot.InitializeAsync();
        }



        public DiscordSocketClient SocketClient { get; }
        private CommandService CmdService { get; }
        public InputHandlers CommandHandler { get; }
        public IServiceProvider Services { get; private set; }
        public ulong BotOwnerId { get; private set; }


        public DiscordBot()
        {
            SocketClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = false,
                LogLevel = LogSeverity.Debug,

            });

            CmdService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,

            });

            Services = SetupServices();

            CommandHandler = new InputHandlers(SocketClient, CmdService, Services);

            

            SocketClient.Log += message => {
                Log.LogService.GetService<DiscordLogger>().Log(message); 
                return Task.CompletedTask;
            };
        }


        

        public async Task InitializeAsync()
        {
            await CmdService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
            await SocketClient.LoginAsync(TokenType.Bot, Config.Config.Instance.DiscordBotToken);
            await SocketClient.StartAsync();
            BotOwnerId = (await SocketClient.GetApplicationInfoAsync()).Owner.Id;
            await CommandHandler.InitializeAsync(BotOwnerId);

            await SocketClient.SetGameAsync("Servers Offline", null, ActivityType.Playing);
            ServerPark.ActiveServerStatusChange += (s, e) => ServerStatusChange((MinecraftServer.IMinecraftServer) s);
            ServerPark.ActiveServerPlayerJoined += (s, e) => ServerStatusChange((MinecraftServer.IMinecraftServer) s);
            ServerPark.ActiveServerPlayerLeft += (s, e) => ServerStatusChange((MinecraftServer.IMinecraftServer) s);

        }

        private async void ServerStatusChange(MinecraftServer.IMinecraftServer server)
        {
            string name = $"{server.ServerName} - {server.OnlinePlayers.Count} active players";
            await SocketClient.SetGameAsync(name, null, ActivityType.Playing);
        }


        private IServiceProvider SetupServices()
            => new ServiceCollection()
            .AddSingleton(SocketClient)
            .AddSingleton(CmdService)
            .BuildServiceProvider();
    }
}
