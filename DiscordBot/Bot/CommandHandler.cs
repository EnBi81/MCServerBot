using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot
{
    internal class CommandHandler
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
            _client.Ready += ReadyEvent;
            _client.SlashCommandExecuted += SlashCommandExecuted;
            _client.InteractionCreated += async arg =>
            {
                SocketInteractionContext context = new SocketInteractionContext(_client, arg);
                await _interactionService.ExecuteCommandAsync(context, _services);
            };

            BotOwnerId = (await _client.GetApplicationInfoAsync()).Owner.Id;
        }

        private async Task SlashCommandExecuted(SocketSlashCommand arg)
        {
            SocketInteractionContext context = new SocketInteractionContext(_client, arg);
            await _interactionService.ExecuteCommandAsync(context, _services);
        }


        private async Task ReadyEvent()
        {
            await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

#if DEBUG
            await _interactionService.RegisterCommandsToGuildAsync(765567760327507979, true);
#else
            await _interactionService.RegisterCommandsGloballyAsync(true);
#endif
        }
    }
}
