using Discord;

namespace DiscordBot.Bot.Helpers
{
    internal static class ButtonHelper
    {
        public static ButtonBuilder CreateProceedButton(string customId, string label) =>
            ButtonBuilder.CreateSuccessButton(label, customId);

        public static ButtonBuilder CreateCancelButton(string customId, string label = "Cancel") => 
            ButtonBuilder.CreateDangerButton(label, customId);

        public static ButtonBuilder CreateLinkButton(string label, string url) =>
            ButtonBuilder.CreateLinkButton(label, url);


        public static MessageComponent JoinButtons(params ButtonBuilder[] buttons)
        {
            var component = new ComponentBuilder();
            var row = new ActionRowBuilder();

            foreach (var button in buttons)
            {
                row.WithButton(button);
            }

            return component.AddRow(row).Build();
        }
    }
}
