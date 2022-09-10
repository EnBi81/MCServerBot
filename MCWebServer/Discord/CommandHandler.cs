using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MCWebServer.Discord.Commands;
using MCWebServer.Discord.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCWebServer.PermissionControll;

namespace MCWebServer.Discord
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private IServiceProvider _services;
        private ulong _botOwnerId;

        public static IReadOnlyDictionary<string, Func<SocketSlashCommand, Task>> Commands { get; } =
            new Dictionary<string, Func<SocketSlashCommand,Task>>()
            {
                ["start-server"] = MinecraftServerCommands.StartServer,
                ["stop-server"] = MinecraftServerCommands.ShutDownServer,
                ["grant-perm"] = PermissionCommands.GrantPermission,
                ["revoke-perm"] = PermissionCommands.RevokePermission,
                ["get-web-url"] = PermissionCommands.GetWebLoginPage,
                ["ping"] = ToolCommands.Ping,
            };


        public CommandHandler(DiscordSocketClient client, CommandService cmdService, IServiceProvider services)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
        } 


        internal async Task InitializeAsync(ulong botOwner)
        {
            _botOwnerId = botOwner;
            
            await _client.SetStatusAsync(UserStatus.Online);

            _client.Ready += async () => await CommandSetup.SetUpSlashCommands(_client, Commands);
            _cmdService.Log += async msg => await LogService.Log.LogAsync(msg);
            _client.SlashCommandExecuted += SlashCommandExecuted;
            _client.SelectMenuExecuted += MenuHandlers.HandleMenu;
        }

        

        private async Task SlashCommandExecuted(SocketSlashCommand arg)
        {
            if (arg.User.IsBot)
                return;

            var userId = arg.User.Id;

            // check if the user has permission
            if (userId != _botOwnerId && !BotPermission.HasPermission(userId))
            { 
                await arg.RespondAsync("You don't have permission to use the commands. Please contact someone who has!", ephemeral: true);
                return;
            }


            // execute command
            var function = Commands[arg.Data.Name];
            await function(arg);
        }
    }
}
