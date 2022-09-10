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


        [Command("Reset All Commands")]
        [OwnerCommand]
        public static async Task ResetAllCommands(SocketSlashCommand command)
        {
            await CommandSetup.RemoveAllCommands(DiscordBot.Bot.SocketClient);
            await CommandSetup.SetUpSlashCommands(DiscordBot.Bot.SocketClient, CommandHandler.Commands, true);

            await command.RespondAsync("Setup Complete");
        }
    }
}
