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
        private readonly DiscordPermission _discordPermissions;


        public ulong BotOwnerId { get; private set; }

        public CommandHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services, DiscordPermission permissions)
        {
            _client = client;
            _interactionService = interactionService;   
            _services = services;
            _discordPermissions = permissions;
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
            if (arg.User.IsBot)
                return;

            if(arg.User.Id != BotOwnerId && !await _discordPermissions.HasPermission(arg.User.Id))
            {
                await arg.RespondAsync("You don't have permission to use this feature.", ephemeral: true);
                return;
            }

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
