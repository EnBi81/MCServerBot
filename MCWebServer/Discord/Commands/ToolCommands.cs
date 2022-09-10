using Discord;
using Discord.WebSocket;
using MCWebServer.Discord.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCWebServer.Discord.Commands
{
    public class ToolCommands
    {
        [Command("Reply pong")]
        public static async Task Ping(SocketSlashCommand command)
        {
            await command.RespondAsync("Pong", ephemeral: true);
        }
    }
}
