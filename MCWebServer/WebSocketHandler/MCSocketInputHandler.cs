using MCWebServer.MinecraftServer;
using MCWebServer.MinecraftServer.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCWebServer.WebSocketHandler
{
    /// <summary>
    /// Handles input received from sockets
    /// </summary>
    public class MCSocketInputHandler
    {
        private readonly MCWebSocket _parent;

        /// <summary>
        /// Collection of the input handlers
        /// </summary>
        public static Dictionary<string, Func<MCWebSocket, JObject?, Task>> Handlers { get; } = new()
        {
            ["toggle"] = ToggleHandler,
            ["add-server"] = AddServerHandler,
            ["remove-server"] = RemoveServerHandler,
            ["rename-server"] = RenameServerHandler,
            ["write-command"] = WriteCommandHandler,
            ["logout"] = LogoutHandler
        };

        

        public MCSocketInputHandler(MCWebSocket parent)        {
            _parent = parent;
        }

        /// <summary>
        /// Handle the input
        /// </summary>
        /// <param name="requestName"></param>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task HandleInput(string? requestName, JObject? requestData)
        {
            if (string.IsNullOrEmpty(requestName))
            {
                await _parent.SendMessage("Request null or invalid.");
                return;
            }

            if (!Handlers.TryGetValue(requestName, out var handler))
            {
                await _parent.SendMessage("Could not recognize the request name.");
                return;
            }

            await handler(_parent, requestData);
        }



        /// <summary>
        /// Handles the toggle request from the client.
        /// </summary>
        /// <param name="sender">Parent class to make the events</param>
        /// <param name="data">
        /// Expected to look something like this:
        /// {
        ///    "server-name": "server-name" // ignored when server is running
        /// }
        /// </param>
        /// <returns></returns>
        public static async Task ToggleHandler(MCWebSocket sender, JObject? data)
        {
            IMinecraftServer activeServer = ServerPark.ActiveServer;
            string username = sender.DiscordUser.Username;

            try
            {
                if (activeServer.IsRunning)
                {
                    ServerPark.StopActiveServer(username);
                }
                else
                {
                    if (data is null)
                        throw new Exception("data key must not have a null value!");

                    string? serverName = data["server-name"]?.Value<string>();

                    if (serverName is null)
                        throw new Exception("server-name must not be null!");

                    ServerPark.StartServer(serverName, username);
                }

            } catch (Exception e)
            {
                string mess = MessageFormatter.Log(e.Message, (int)LogMessage.LogMessageType.Error_Message);
                await sender.SendMessage(mess);
            }
        }

        private static Task AddServerHandler(MCWebSocket arg1, JObject? arg2)
        {
            throw new NotImplementedException();
        }

        private static Task RemoveServerHandler(MCWebSocket arg1, JObject? arg2)
        {
            throw new NotImplementedException();
        }

        private static Task RenameServerHandler(MCWebSocket arg1, JObject? arg2)
        {
            throw new NotImplementedException();
        }

        private static Task WriteCommandHandler(MCWebSocket arg1, JObject? arg2)
        {
            throw new NotImplementedException();
        }

        private static Task LogoutHandler(MCWebSocket arg1, JObject? arg2)
        {
            throw new NotImplementedException();
        }
    }
}
