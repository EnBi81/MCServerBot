using Discord;

namespace MCWebServer.Discord.Helpers
{
    public class MenuHelpers
    {
        public const string StartServerMenuId = "start-server-menu";

        /// <summary>
        /// Create a menu of the server list.
        /// </summary>
        /// <returns></returns>
        public static MessageComponent CreateServerListMenu(string withMenuId)
        {
            var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select an option")
            .WithCustomId(withMenuId)
            .WithMinValues(1)
            .WithMaxValues(1);

            foreach (var server in MinecraftServer.ServerPark.MCServers.Values)
                menuBuilder.AddOption(server.ServerName, server.ServerName, $"Disk space: {server.StorageSpace}");

            var builder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder);

            return builder.Build();
        }
    }
}
