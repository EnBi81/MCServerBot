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
        public CommandHandler CommandHandler { get; }
        public IServiceProvider Services { get; private set; }


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

            CommandHandler = new CommandHandler(SocketClient, CmdService, Services);

            ServerPark.ActiveServerStatusChange += ServerStatusChange;

            SocketClient.Log += message => {
                Log.LogService.GetService<DiscordLogger>().Log(message); 
                return Task.CompletedTask;
            };
        }


        private async void ServerStatusChange(object? sender, ServerStatus e)
        {
            string name = e.DisplayString();
            await SocketClient.SetGameAsync(name, null, ActivityType.Playing);
        }

        public async Task InitializeAsync()
        {
            await CmdService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
            await SocketClient.LoginAsync(TokenType.Bot, Config.Config.Instance.DiscordBotToken);
            await SocketClient.StartAsync();
            await CommandHandler.InitializeAsync();

            await SocketClient.SetGameAsync("Server Offline", null, ActivityType.Playing);
        }


        private IServiceProvider SetupServices()
            => new ServiceCollection()
            .AddSingleton(SocketClient)
            .AddSingleton(CmdService)
            .BuildServiceProvider();
    }
}
