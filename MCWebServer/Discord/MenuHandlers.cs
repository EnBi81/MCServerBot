using Discord.WebSocket;
using MCWebServer.Discord.Helpers;
using MCWebServer.MinecraftServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCWebServer.Discord
{
    public class MenuHandlers
    {
        private static IReadOnlyDictionary<string, Func<SocketMessageComponent, Task>> Handlers { get; } =
            new Dictionary<string, Func<SocketMessageComponent, Task>>()
            {
                [MenuHelpers.StartServerMenuId] = ServerStartMenu,
            };

        public static async Task HandleMenu(SocketMessageComponent arg)
        {
            string menuId = arg.Data.CustomId;
            var handler = Handlers[menuId];
            
            await handler(arg);
        }

        

        public static async Task ServerStartMenu(SocketMessageComponent arg)
        {
            string serverName = arg.Data.Value;
            
            try
            {
                ServerPark.StartServer(serverName, arg.User.Username);
                await arg.RespondAsync("Starting server " + serverName);
            } catch (Exception e)
            {
                await arg.RespondAsync("Server starting failed: " + e.Message);
            }
            
        }
    }
}
