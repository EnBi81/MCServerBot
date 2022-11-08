using Loggers;
using Application.Minecraft;

using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Minecraft.MinecraftServers;

namespace Application.WebSocketHandler
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
                await SendBackErrorMessage(_parent, "Request null or invalid.");
                return;
            }

            if (!Handlers.TryGetValue(requestName, out var handler))
            {
                await SendBackErrorMessage(_parent, "Could not recognize the request name.");
                return;
            }


            LogService.GetService<WebLogger>().Log("socket", $"Command received from {_parent.DiscordUser.Username}, command: {requestData}");

            try
            {
                await handler(_parent, requestData);
            } catch (Exception e)
            {
                await SendBackErrorMessage(_parent, e.Message);
            }
        }

        /// <summary>
        /// Sends an error message to the socket.
        /// </summary>
        /// <param name="sendTo">send the error message to.</param>
        /// <param name="message">error message</param>
        /// <returns></returns>
        public static async Task SendBackErrorMessage(MCWebSocket sendTo, string message) =>
            await sendTo.SendBackErrorMessage(message);


        /// <summary>
        /// Handles the toggle request from the client.
        /// </summary>
        /// <param name="sender">Parent class to make the events</param>
        /// <param name="data">
        /// Expected to look something like this:
        /// {
        ///    "server-name": "serverName" // ignored when server is running
        /// }
        /// </param>
        /// <returns></returns>
        public static Task ToggleHandler(MCWebSocket sender, JObject? data)
        {
            IMinecraftServer? activeServer = ServerPark.ActiveServer;


            string username = sender.DiscordUser.Username;

            
            if (activeServer?.IsRunning ?? false)
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

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds a new server to the server list
        /// </summary>
        /// <param name="sender">Parent class to make the events</param>
        /// <param name="data">
        /// Expected to look something like this:
        /// {
        ///    "server-name": "serverName"
        /// }
        /// </param>
        /// <returns></returns>
        private static Task AddServerHandler(MCWebSocket sender, JObject? data)
        {
            if (data is null)
                throw new Exception("data key must not have a null value!");

            string? serverName = data["server-name"]?.Value<string>();

            if (serverName is null)
                throw new Exception("server-name must not be null!");

            ServerPark.CreateServer(serverName);

            return Task.CompletedTask;
        }


        /// <summary>
        /// Removes a minecraft server from the system.
        /// </summary>
        /// <param name="sender">Parent class to make the events</param>
        /// <param name="data">
        /// Expected to look something like this:
        /// {
        ///    "server-name": "serverName"
        /// }
        /// </param>
        /// <returns></returns>
        private static Task RemoveServerHandler(MCWebSocket sender, JObject? data)
        {
            if (data is null)
                throw new Exception("data key must not have a null value!");

            string? serverName = data["server-name"]?.Value<string>();

            if (serverName is null)
                throw new Exception("server-name must not be null!");

            ServerPark.DeleteServer(serverName);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Renames a minecraft server.
        /// </summary>
        /// <param name="sender">Parent class to make the events</param>
        /// <param name="data">
        /// Expected to look something like this:
        /// {
        ///    "old-name": "serverName"
        ///    "new-name": "serverName"
        /// }
        /// </param>
        /// <returns></returns>
        private static Task RenameServerHandler(MCWebSocket sender, JObject? data)
        {
            if (data is null)
                throw new Exception("data key must not have a null value!");

            string? oldName = data["old-name"]?.Value<string>();
            string? newName = data["new-name"]?.Value<string>();

            if (oldName is null)
                throw new Exception("old-name must not be null!");
            if (newName is null)
                throw new Exception("new-name must not be null!");

            ServerPark.RenameServer(oldName, newName);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Writes a command to the currenty active server.
        /// </summary>
        /// <param name="sender">Parent class to make the events</param>
        /// <param name="data">
        /// Expected to look something like this:
        /// {
        ///    "server-name": "name"
        ///    "command": "command-data"
        /// }
        /// </param>
        /// <returns></returns>
        private static Task WriteCommandHandler(MCWebSocket sender, JObject? data)
        {
            if (data is null)
                throw new Exception("data key must not have a null value!");

            string? serverName = data["server-name"]?.Value<string>();

            if(serverName is null || serverName == ServerPark.ActiveServer?.ServerName)
            {
                throw new Exception("Server is not running.");
            }

            string? command = data["command"]?.Value<string>();

            if (command is null)
                throw new Exception("command must not be null!");

            ServerPark.ActiveServer?.WriteCommand(command, sender.DiscordUser.Username);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Closes the Socket
        /// </summary>
        /// <param name="sender">Parent class to make the events</param>
        /// <param name="data">none</param>
        /// <returns></returns>
        private static async Task LogoutHandler(MCWebSocket sender, JObject? data)
        {
            LogService.GetService<WebLogger>().Log("socket", "Logout request from " + sender.DiscordUser.Username);
            await sender.Close();
        }
    }
}
