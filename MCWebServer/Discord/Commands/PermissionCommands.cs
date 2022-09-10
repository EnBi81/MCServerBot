using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Linq;
using MCWebServer.PermissionControll;
using MCWebServer.Discord.Helpers;

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
                var embed = EmbedHelper.CreateTitleEmbed("No user provided :x:");
                await command.RespondAsync(embed: embed, ephemeral: true);
                return;
            }

            ulong id = user.Id;

            try
            {
                BotPermission.GrantPermission(id, user);
                var embed = EmbedHelper.CreateTitleEmbed($"Permission Granted for {user.Username} :white_check_mark:", author: user);
                await command.RespondAsync(embed: embed, ephemeral: true);
            }
            catch (Exception)
            {
                var embed = EmbedHelper.CreateTitleEmbed($"Permission for {user.Username} is already granted :x:", author: user);
                await command.RespondAsync(embed: embed, ephemeral: true);
            }
        }

        [Command("Revoke Permission for a User")]
        [CommandOption("user", "User to revoke permission from", ApplicationCommandOptionType.User, true)]
        public static async Task RevokePermission(SocketSlashCommand command)
        {
            var option = command.Data.Options.First();

            if (option.Value is not IUser user)
            {
                var embed = EmbedHelper.CreateTitleEmbed("No user provided :x:");
                await command.RespondAsync(embed: embed, ephemeral: true);
                return;
            }

            ulong id = user.Id;
            if (id == Config.Config.Instance.DiscordBotOwner)
            {
                var embed = EmbedHelper.CreateTitleEmbed("You cannot remove the bot owner, darling ;)", author: user);
                await command.RespondAsync(embed: embed, ephemeral: true);
                return;
            }

            try
            {
                BotPermission.RevokePermission(id);
                var embed = EmbedHelper.CreateTitleEmbed($"Permission Revoked for {user.Username} :white_check_mark:", author: user);
                await command.RespondAsync(embed: embed, ephemeral: true);
            }
            catch (Exception)
            {
                var embed = EmbedHelper.CreateTitleEmbed($"{user.Username} does not have active permission:x:", author: user);
                await command.RespondAsync(embed: embed, ephemeral: true);
            }
        }

        [Command("Get the login link to the server manager page")]
        public static async Task GetWebLoginPage(SocketSlashCommand command)
        {
            var id = command.User.Id;
            try
            {
                string code = WebsitePermission.GetCode(id);

                string hamachiSite = WebsitePermission.CreatePrivateUrl(WebsitePermission.WebsiteHamachiUrl, code);
                string publicDomain = WebsitePermission.CreatePrivateUrl(WebsitePermission.WebsiteDomainUrl, code);

                string description = $"Your code is: \n?code={code}\n" +
                    $"[{WebsitePermission.WebsiteHamachiUrl}]({hamachiSite})\n" +
                    $"[{WebsitePermission.WebsiteDomainUrl}]({publicDomain})\n";
               

                if(id == Config.Config.Instance.DiscordBotOwner)
                {
                    string localSite = WebsitePermission.CreatePrivateUrl(WebsitePermission.WebsiteLocalUrl, code);

                    description += $"[{WebsitePermission.WebsiteLocalUrl}]({localSite})\n";
                }

                description += "(Please don't share this link with anyone!)";

                var embed = EmbedHelper.CreateTitleEmbed("Unique Link to website", description: description, includeWebsiteLink: false);
                await command.RespondAsync(embed: embed, ephemeral: true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
