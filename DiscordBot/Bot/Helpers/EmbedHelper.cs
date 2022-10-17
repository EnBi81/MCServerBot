using Discord;
using Application.PermissionControll;

namespace DiscordBot.Bot.Helpers
{
    public static class EmbedHelper
    {
        public static Embed CreateTitleEmbed(string text, string url = null, IUser author = null, string description = null, bool includeWebsiteLink = true)
        {
            var builder = new EmbedBuilder()
                .WithTitle(text);

            if (url != null)
                builder.WithUrl(url);

            if (author != null)
                builder.WithAuthor(author);

            if (description != null)
                builder.WithDescription(description);

            return includeWebsiteLink
                ? builder.AppendLinkField()
                : builder.Build();
        }


        private static Embed AppendLinkField(this EmbedBuilder builder)
        {
            var field = new EmbedFieldBuilder()
                .WithIsInline(false)
                .WithName("Websites")
                .WithValue($"[{WebsitePermission.WebsiteHamachiUrl}]({WebsitePermission.WebsiteHamachiUrl})\n" +
                           $"[{WebsitePermission.WebsiteDomainUrl}]({WebsitePermission.WebsiteDomainUrl})");


            return builder.WithFields(field).Build();
        }
    }
}
