using Application.Minecraft;
using Discord;

namespace DiscordBot.Bot.Helpers
{
    public class MenuHelpers
    {
        //public const string StartServerMenuId = "start-server-menu";
        //public const string RenameServerMenuId = "rename-server-menu";
        //public const string DeleteServerMenuId = "delete-server-menu";

        ///// <summary>
        ///// Create a menu of the server list.
        ///// </summary>
        ///// <returns></returns>
        //public static MessageComponent CreateServerListMenu(string withMenuId)
        //{
        //    var menuBuilder = ServerListMenuBuilder(withMenuId);

        //    var builder = new ComponentBuilder()
        //        .WithSelectMenu(menuBuilder);

        //    return builder.Build();
        //}

        //public static SelectMenuBuilder ServerListMenuBuilder(string withMenuId)
        //{
        //    var menuBuilder = new SelectMenuBuilder()
        //    .WithPlaceholder("Select an option")
        //    .WithCustomId(withMenuId)
        //    .WithMinValues(1)
        //    .WithMaxValues(1)
        //    .WithPlaceholder("Select a Server");

        //    foreach (var server in ServerPark.MCServers.Values)
        //        menuBuilder.AddOption(server.ServerName, server.ServerName, $"Disk space: {server.StorageSpace}");

        //    return menuBuilder;
        //}
    }
}
