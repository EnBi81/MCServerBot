using Discord.Interactions;
using Discord.WebSocket;
using Loggers;
using System.Reflection;

namespace DiscordBot.Bot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _services;

        public ulong BotOwnerId { get; private set; }

        public CommandHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services)
        {
            _client = client;
            _interactionService = interactionService;   
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

            _client.Ready += () => RegisterCommands(false);
            _client.InteractionCreated += ExecuteSocketInteraction;

            BotOwnerId = (await _client.GetApplicationInfoAsync()).Owner.Id;
        }

        private async Task ExecuteSocketInteraction(SocketInteraction arg) 
        {
            SocketInteractionContext context = new SocketInteractionContext(_client, arg);
            IResult result = await _interactionService.ExecuteCommandAsync(context, _services);

            if(!result.IsSuccess)
            {
                LogService.GetService<DiscordLogger>().LogError(result.Error + " - " + result.ErrorReason);
            }
        }

        private async Task RegisterCommands(bool global)
        {
            try
            {
                if (global)
                    await _interactionService.RegisterCommandsGloballyAsync(true);
                else
                    await _interactionService.RegisterCommandsToGuildAsync(765567760327507979, true);
            }
            catch (Exception e)
            {
                LogService.GetService<DiscordLogger>().LogFatal(e.Message);
            }
        }
    }
}
