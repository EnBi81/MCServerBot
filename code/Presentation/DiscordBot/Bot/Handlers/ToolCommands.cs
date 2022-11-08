using Discord.Interactions;

namespace DiscordBot.Bot.Handlers
{
    public class ToolCommands : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("ping", "Reply pong")]
        public async Task Ping()
        {
            await RespondAsync("pong", ephemeral: true);
        }
    }
}
