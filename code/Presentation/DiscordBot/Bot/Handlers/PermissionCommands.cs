using Discord;
using DiscordBot.Bot.Helpers;
using DiscordBot.PermissionControll;
using Discord.Interactions;
using DataStorage.Interfaces;
using HamachiHelper;
using Application.DAOs.Database;

namespace DiscordBot.Bot.Handlers
{
    internal class PermissionCommands : MCInteractionModuleBase
    {
        private readonly DiscordPermission _discordPermission;

        public PermissionCommands(IDiscordDataAccess discordEventRegister, DiscordPermission discordPermission) : base(discordEventRegister)
        {
            _discordPermission = discordPermission;
        }


        // do refresh here
        [SlashCommand("refresh-user-info", "Refreshes user info in the database such as username and profile pic")]
        public async Task RefreshUser()
        {
            await _discordPermission.RefreshUser(Context.User);
            await RespondAsync("User successfully refreshed!");
        }

        [SlashCommand("grant-permission", "Grant Permission for a User")]
        public async Task GrantPermission([Summary("user", "User to give permission to")] IUser user)
        {
            try
            {
                await _discordPermission.GrantPermission(Context.User.Id, user);
                await RespondAsync($"Permission **Granted** for user **{user.Username}**", ephemeral: true);
            }
            catch (Exception e)
            {
                await RespondAsync(e.Message, ephemeral: true);
            }
        }

        [SlashCommand("revoke-permission", "Revoke Permission for a User")]
        public async Task RevokePermission([Summary("user", "User to revoke permission from")] IUser user)
        {
            try
            {
                await _discordPermission.RevokePermission(Context.User.Id, user);
                await RespondAsync($"Permission **Revoked** for **{user.Username}**", ephemeral: true);
            }
            catch (Exception e)
            {
                await RespondAsync(e.Message, ephemeral: true); 
            }
        }

        [SlashCommand("get-web-url", "Get the login link to the server manager page")]
        public async Task GetWebLoginPage()
        {
            var user = await GetUser();

            if(user == null)
            {
                await RespondAsync("You don't have permission to do this :(((");
                return;
            }

            string createLink = $"https://{HamachiClient.Address}/servers?token={user.WebAccessToken}";
            var button = ButtonHelper.CreateLinkButton("Website", createLink);

            await RespondAsync("Here is your link", components: ButtonHelper.JoinButtons(button));
        }
    }
}
