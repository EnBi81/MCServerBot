using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace MCWebServer.Discord.Commands
{
    internal class PermissionCommands
    {
        [Command("Grant Permission for a User")]
        [CommandOption("user", "User to grant permission for", ApplicationCommandOptionType.User, true)]
        public static async Task GrantPermission(SocketSlashCommand command)
        {
            var option = command.Data.Options.First();

            if (option.Value is not IUser user)
            {
                await command.RespondAsync("No user provided :x:", ephemeral: true);
                return;
            }

            ulong id = user.Id;

            try
            {
                CommandPermission.GrantPermission(id);
                await command.RespondAsync($"Permission Granted for {user.Username} :white_check_mark:");
            }
            catch (Exception)
            {
                await command.RespondAsync($"Permission for {user.Username} is already granted :x:", ephemeral: true);
            }
        }

        [Command("Revoke Permission for a User")]
        [CommandOption("user", "User to revoke permission from", ApplicationCommandOptionType.User, true)]
        public static async Task RevokePermission(SocketSlashCommand command)
        {
            var option = command.Data.Options.First();

            if (option.Value is not IUser user)
            { 
                await command.RespondAsync("No user provided :x:", ephemeral: true);
                return;
            }

            ulong id = user.Id;

            try
            {
                CommandPermission.RevokePermission(id);
                await command.RespondAsync($"Permission Revoked for {user.Username} :white_check_mark:", ephemeral: true);
            }
            catch (Exception)
            {
                await command.RespondAsync($"{user.Username} does not have active permission:x:", ephemeral: true);
            }
        }
    }
}
