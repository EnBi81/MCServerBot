using Discord;

namespace DiscordBot.Bot.Helpers
{
    internal static class ButtonHelper
    {
        public static ButtonComponent CreateProceedButton(string customId, string label) =>
            ButtonBuilder.CreateSuccessButton(label, customId).Build();

        public static ButtonComponent CreateCancelButton(string customId, string label = "Cancel") => 
            ButtonBuilder.CreateDangerButton(label, customId).Build();

        public static ButtonComponent CreateLinkButton(string label, string url) =>
            ButtonBuilder.CreateLinkButton(label, url).Build();


        public static MessageComponent JoinButtons(params ButtonComponent[] buttons) => ComponentBuilder.FromComponents(buttons).Build();
    }
}
