using Discord;
using Discord.WebSocket;
using DiscordBot.Discord.Helpers;
using Application.MinecraftServer;

namespace DiscordBot.Discord.Commands
{
    internal class MinecraftServerCommands
    {
        [Command("Start the minecraft server")]
        public static async Task StartServer(SocketSlashCommand command)
        {
            MessageComponent menu = MenuHelpers.CreateServerListMenu(MenuHelpers.StartServerMenuId);
            await command.RespondAsync("Please select a server to start: ", components: menu);
        }

        [Command("Stop the minecraft server")]
        public static async Task ShutDownServer(SocketSlashCommand command)
        {
            try
            {
                ServerPark.StopActiveServer(command.User.Username);
                await command.RespondAsync($"Shutting Down **{ServerPark.ActiveServer?.ServerName}**");
            }
            catch (Exception ex)
            {
                var embed = EmbedHelper.CreateTitleEmbed("Shutting Down failed: " + ex.Message, author: command.User);
                await command.RespondAsync(embed: embed, ephemeral: true);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }


        [Command("Create a new server")]
        [CommandOption("server-name", "Name of the new server.", ApplicationCommandOptionType.String, true)]
        public static async Task CreateServer(SocketSlashCommand command)
        {
            string? serverName = command.Data.Options.First().Value.ToString();

            if(serverName == null)
            {
                await command.RespondAsync("**No server name specified!**");
                return;
            }
            
            try
            {
                ServerPark.CreateServer(serverName);
                await command.RespondAsync($"Server **{serverName}** has been created");
            } catch (Exception e)
            {
                await command.RespondAsync($"Server **{serverName}** cannot be created: **{e.Message}**");
            }
        }

        [Command("Rename a server")]
        public static async Task RenameServer(SocketSlashCommand command)
        {
            try
            {
                var menu = MenuHelpers.CreateServerListMenu(MenuHelpers.RenameServerMenuId);
                await command.RespondAsync("Please select a server to rename:", components: menu);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        [Command("Delete a server")]
        public static async Task DeleteServer(SocketSlashCommand command)
        {
            var menu = MenuHelpers.CreateServerListMenu(MenuHelpers.DeleteServerMenuId);
            await command.RespondAsync("Select the server you want to delete: ", components: menu);
        }
    }
}
