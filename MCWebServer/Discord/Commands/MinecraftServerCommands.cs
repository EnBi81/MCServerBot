using Discord;
using Discord.WebSocket;
using MCWebServer.Discord.Helpers;
using MCWebServer.MinecraftServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebServer.Discord.Commands
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

                var embed = EmbedHelper.CreateTitleEmbed("Shutting Down Server", author: command.User);
                await command.RespondAsync(embed: embed);
            }
            catch (Exception ex)
            {
                var embed = EmbedHelper.CreateTitleEmbed("Shutting Down failed: " + ex.Message, author: command.User);
                await command.RespondAsync(embed: embed, ephemeral: true);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
