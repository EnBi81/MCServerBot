using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Application.Minecraft;
using Microsoft.Extensions.DependencyInjection;
using Loggers;
using Application.Minecraft.MinecraftServers;
using Discord.Interactions;

namespace DiscordBot.Bot
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


        

        public DiscordSocketClient SocketClient { get; }
        internal CommandHandler CommandHandler { get; }
        internal IServiceProvider Services { get; private set; }
        

        private readonly string _token;

        public DiscordBot(string token)
        {
            _token = token;

            Services = SetupServices();
            SocketClient = Services.GetRequiredService<DiscordSocketClient>();
            CommandHandler = Services.GetRequiredService<CommandHandler>();

            
        }



        public async Task InitializeAsync()
        {
            SocketClient.Log += message => {
                LogService.GetService<DiscordLogger>().Log(message);
                return Task.CompletedTask;
            };

            await SocketClient.LoginAsync(TokenType.Bot, _token);
            await SocketClient.StartAsync();

            await CommandHandler.InitializeAsync();

            await SocketClient.SetGameAsync("Servers Offline", null, ActivityType.Playing);
            //ServerPark.ActiveServerStatusChange += (s, e) => ServerStatusChange((IMinecraftServer) s!);
            //ServerPark.ActiveServerPlayerJoined += (s, e) => ServerStatusChange((IMinecraftServer) s!);
            //ServerPark.ActiveServerPlayerLeft += (s, e) => ServerStatusChange((IMinecraftServer) s!);
        }

        private async void ServerStatusChange(IMinecraftServer server)
        {
            string name = $"{server.ServerName} - {server.OnlinePlayers.Count} active players";
            await SocketClient.SetGameAsync(name, null, ActivityType.Playing);
        }


        private IServiceProvider SetupServices()
            => new ServiceCollection()
            .AddSingleton(IServerPark.Instance)
            .AddSingleton(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = false,
                LogLevel = LogSeverity.Debug,
            })
            .AddSingleton(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = false,
                DefaultRunMode = Discord.Commands.RunMode.Async,
            })
            .AddSingleton(new InteractionServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                AutoServiceScopes = false,
                EnableAutocompleteHandlers = true,
                DefaultRunMode = Discord.Interactions.RunMode.Async,
            })
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();
    }
}
