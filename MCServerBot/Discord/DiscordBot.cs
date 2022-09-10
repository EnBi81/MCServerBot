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

namespace MCWebServer.Discord
{
    internal class DiscordBot
    {
        public DiscordSocketClient SocketClient { get; }
        private CommandService CmdService { get; }
        private CommandHandler CommandHandler { get; }
        public IServiceProvider Services { get; private set; }

        public LogService LogService { get; } = LogService.Log;

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

            ServerPark.Keklepcso.StatusChange += ServerStatusChange;
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
            .AddSingleton(LogService)
            .BuildServiceProvider();
    }
}
