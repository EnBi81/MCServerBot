using Discord.WebSocket;
using MCWebServer.Discord.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Discord;
using System.Linq;
using MCWebServer.MinecraftServer;

namespace MCWebServer.Discord.Handlers
{
    public class ModalHandlers
    {
        private static IReadOnlyDictionary<string, Func<SocketModal, Task>> Handlers { get; } =
            new Dictionary<string, Func<SocketModal, Task>>()
            {
                [ModalHelpers.DeleteServerModalId] = DeleteServerModal,
            };

        /// <summary>
        /// Selects the correct handler for the modal and executes it
        /// </summary>
        /// <param name="arg">The menu to handle</param>
        /// <returns></returns>
        public static async Task HandleModal(SocketModal arg)
        {
            string menuId = arg.Data.CustomId;

            if (!Handlers.TryGetValue(menuId, out Func<SocketModal, Task> handler))
            {
                await arg.RespondAsync("Unrecognized Modal");
                return;
            }

            await handler(arg);
        }


        public static async Task DeleteServerModal(SocketModal arg)
        {
            var components = arg.Data.Components;
            
            if(components == null || components.Count == 0)
            {
                await arg.RespondAsync("Incorrect Modal Form");
                return;
            }

            string serverNameInput = components.First().Value;

            try
            {
                ServerPark.DeleteServer(serverNameInput);
                await arg.RespondAsync($"Server **{serverNameInput}** deleted.");
            }
            catch (Exception e)
            {
                await arg.RespondAsync($"Error occured: **{e.Message}**");
            }
        }
    }
}
