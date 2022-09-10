using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Commands
{
    internal class TestCommands
    {
        [Command("Test Command")]
        public static async Task TestCommand(SocketSlashCommand command)
        {
            var menuBuilder = new SelectMenuBuilder()
        .WithPlaceholder("Select an option")
        .WithCustomId("menu-1")
        .WithMinValues(1)
        .WithMaxValues(1)
        .AddOption("Option A", "opt-a", "Option B is lying!")
        .AddOption("Option B", "opt-b", "Option A is telling the truth!");

            var builder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder);

            await command.RespondAsync("Whos really lying?", components: builder.Build());
        }
    }
}
