using Discord.Interactions;

namespace DiscordBot.Bot.Handlers
{
    public class ButtonReactions : InteractionModuleBase<SocketInteractionContext>
    {
        [ComponentInteraction("delete-cancel")]
        public async Task DeleteCancel()
        {
            await RespondAsync("Hello bello");
        }

        [ComponentInteraction("delete-proceed")]
        public async Task DeleteProceed()
        {
            await RespondAsync("Hello bello");
        }
    }
}
