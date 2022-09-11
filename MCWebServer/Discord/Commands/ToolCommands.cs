using Discord;
using Discord.WebSocket;
using MCWebServer.Discord.Handlers;
using MCWebServer.Discord.Helpers;
using System.Collections.Generic;
using System.Linq;
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


        [Command("Reset Commands")]
        [CommandOption("full-reset", "Everything from scratch", ApplicationCommandOptionType.Boolean, false)]
        public static async Task ResetAllCommands(SocketSlashCommand command)
        {
            bool fullReset = false;
            if(command.Data.Options.Count > 0)
            {
                fullReset = (bool) command.Data.Options.First().Value;
            }

            await command.DeferAsync();


            await CommandSetup.RemoveCommands(DiscordBot.Bot.SocketClient, fullReset);
            await CommandSetup.SetUpSlashCommands(DiscordBot.Bot.SocketClient, CommandHandlers.Commands, fullReset);

            await command.FollowupAsync("Setup Complete");
        }
    }
}
