using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Services;
using DiscordBot.Discord.Handlers;
using DiscordBot.PermissionControll;

namespace DiscordBot.Discord
{
    internal class InputHandlers
    {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private IServiceProvider _services;
        private ulong _botOwnerId;

        


        public InputHandlers(DiscordSocketClient client, CommandService cmdService, IServiceProvider services)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
        } 


        internal async Task InitializeAsync(ulong botOwner)
        {
            _botOwnerId = botOwner;
            
            await _client.SetStatusAsync(UserStatus.Online);

            //_client.Ready += async () => await CommandSetup.SetUpSlashCommands(_client, CommandHandlers.Commands, true);
            _cmdService.Log += async msg => await LogService.Log.LogAsync(msg);
            _client.SlashCommandExecuted += SlashCommandExecuted;
            _client.SelectMenuExecuted += SelectMenuExecuted;
            _client.ModalSubmitted += ModalSubmitted;
        }

        private bool HasPermission(SocketUser user)
        {
            if (user.IsBot)
                return false;

            var userId = user.Id;

            // check if the user has permission
            return userId == _botOwnerId || BotPermission.HasPermission(userId);
        }

        private async Task SlashCommandExecuted(SocketSlashCommand arg)
        {
            if(!HasPermission(arg.User))
            {
                await arg.RespondAsync("You do not have permission to use this command.", ephemeral: true);
                return;
            }

            await CommandHandlers.HandleCommand(arg);
        }

        private async Task SelectMenuExecuted(SocketMessageComponent arg)
        {
            if (!HasPermission(arg.User))
            {
                await arg.RespondAsync("You do not have permission to use this menu.", ephemeral: true);
                return;
            }

            await MenuHandlers.HandleMenu(arg);
        }

        private async Task ModalSubmitted(SocketModal arg)
        {
            if (!HasPermission(arg.User))
            {
                await arg.RespondAsync("You do not have permission to use this modal.", ephemeral: true);
                return;
            }
            await ModalHandlers.HandleModal(arg);
        }
    }
}
