using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Application.MinecraftServer;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Loggers;
using System.Net.NetworkInformation;

namespace DiscordBot.Discord
{
    /// <summary>
    /// Main part of the Discord Bot.
    /// </summary>
    public class DiscordBot
    {
        private static DiscordBot? _instance;

        /// <summary>
        /// Bot instance.
        /// </summary>
        public static DiscordBot Bot { get => _instance ?? throw new Exception("Discord Bot is not initialized"); }

        /// <summary>
        /// Initialize a Discord Bot.
        /// </summary>
        public static async Task Initialize(string token)
        {
            if (_instance != null)
                return;

            _instance = new DiscordBot(token);
            await Bot.InitializeAsync();
        }


        public ulong BotOwnerId { get; private set; }
        public DiscordSocketClient SocketClient { get; }
        internal CommandService CmdService { get; }

        internal InputHandlers CommandHandler { get; }
        internal IServiceProvider Services { get; private set; }
        

        private readonly string _token;

        public DiscordBot(string token)
        {
            _token = token;

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
                LogService.GetService<DiscordLogger>().Log(message); 
                return Task.CompletedTask;
            };
        }


        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            await CmdService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
            await SocketClient.LoginAsync(TokenType.Bot, _token);
            await SocketClient.StartAsync();
            BotOwnerId = (await SocketClient.GetApplicationInfoAsync()).Owner.Id;
            await CommandHandler.InitializeAsync(BotOwnerId);

            await SocketClient.SetGameAsync("Servers Offline", null, ActivityType.Playing);
            ServerPark.ActiveServerStatusChange += (s, e) => ServerStatusChange((IMinecraftServer) s!);
            ServerPark.ActiveServerPlayerJoined += (s, e) => ServerStatusChange((IMinecraftServer) s!);
            ServerPark.ActiveServerPlayerLeft += (s, e) => ServerStatusChange((IMinecraftServer) s!);

        }

        private async void ServerStatusChange(IMinecraftServer server)
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
