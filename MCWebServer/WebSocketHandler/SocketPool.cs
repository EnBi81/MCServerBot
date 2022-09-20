using MCWebServer.Log;
using MCWebServer.PermissionControll;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MCWebServer.MinecraftServer;
using MCWebServer.MinecraftServer.EventHandlers;
using MCWebServer.MinecraftServer.Enums;

namespace MCWebServer.WebSocketHandler
{
    /// <summary>
    /// SocketPool stores and takes care of all the incoming and established websockets.
    /// </summary>
    public class SocketPool
    {
        /// <summary>
        /// Single SocketPool reference.
        /// </summary>
        public static SocketPool SocketPoolInstance { get; } = new SocketPool();




        /// <summary>
        /// Store all the sockets.
        /// </summary>
        private ICollection<MCWebSocket> Sockets { get; } = new List<MCWebSocket>();

        /// <summary>
        /// Initializes the SocketPool.
        /// </summary>
        private SocketPool()
        {
            LogService.GetService<WebLogger>().Log("socket-pool", "Socket Pool Initalizing");
            SetupListeners();
            LogService.GetService<WebLogger>().Log("socket-pool", "Socket Pool Initalized");
        }

        #region Base methods

        /// <summary>
        /// Sets up the listeners for the ServerPark.
        /// </summary>
        private void SetupListeners()
        {
            LogService.GetService<WebLogger>().Log("socket-pool", "Setting up listeners");

            ServerPark.ActiveServerChange += ActiveServerChange;
            ServerPark.ActiveServerPlayerLeft += ActiveServerPlayerLeft;
            ServerPark.ActiveServerPlayerJoined += ActiveServerPlayerJoined;
            ServerPark.ActiveServerLogReceived += ActiveServerLogReceived;
            ServerPark.ActiveServerPerformanceMeasured += ActiveServerPerformanceMeasured;
            ServerPark.ActiveServerStatusChange += ActiveServerStatusChange;

            ServerPark.ServerAdded += ServerAdded;
            ServerPark.ServerDeleted += ServerDeleted;
            ServerPark.ServerNameChanged += ServerNameChanged;

            WebsitePermission.PermissionRemoved += PermissionRemoved;

            LogService.GetService<WebLogger>().Log("socket-pool", "Listeners have been set up");
        }

        


        /// <summary>
        /// Adds a socket to the SocketPool
        /// </summary>
        /// <param name="code">code of the user.</param>
        /// <param name="socket">captured websocket.</param>
        /// <returns></returns>
        public async Task AddSocket(string code, WebSocket socket)
        {
            var user = WebsitePermission.GetUser(code);
            LogService.GetService<WebLogger>().Log("socket-pool", "New socket received from " + user.Username);

            MCWebSocket socketHandler = new MCWebSocket(socket, user, code);
            var initTask = socketHandler.Initialize();

            RegisterSocket(socketHandler);

            await initTask;
        }


        /// <summary>
        /// Broadcast a message to the connected sockets.
        /// </summary>
        /// <param name="message">Message to broadcast.</param>
        /// <param name="code">user's code to broadcast the message to. If not specified, it broadcasts the message to all the sockets.</param>
        /// <returns></returns>
        public async Task BroadcastMessage(string message, string? code = null)
        {
            List<Task> tasks = new();

            var socketsToBroadcast = code == null ? GetAllSockets() : GetAllSockets(code);

            foreach (var socket in socketsToBroadcast)
                tasks.Add(socket.SendMessage(message));

            foreach (var task in tasks)
                await task;
        }

        #endregion


        #region Listeners
        private async void ActiveServerChange(object? sender, ValueEventArgs<IMinecraftServer> e)
        {
            string newActiveServer = e.NewValue.ServerName;
            string mess = MessageFormatter.ActiveServerChange(newActiveServer);

            await BroadcastMessage(mess);
        }

        private async void ActiveServerPlayerLeft(object? sender, ValueEventArgs<MinecraftPlayer> e)
        {
            MinecraftPlayer player = e.NewValue;
            string mess = MessageFormatter.PlayerLeft(player.Username);

            await BroadcastMessage(mess);
        }

        private async void ActiveServerPlayerJoined(object? sender, ValueEventArgs<MinecraftPlayer> e)
        {
            MinecraftPlayer player = e.NewValue;
            if (player.OnlineFrom is null)
                return;

            string mess = MessageFormatter.PlayerJoin(player.Username, player.OnlineFrom.Value, player.PastOnline);

            await BroadcastMessage(mess);
        }

        private async void ActiveServerLogReceived(object? sender, ValueEventArgs<LogMessage> e)
        {
            LogMessage message = e.NewValue;
            string mess = MessageFormatter.Log(message.Message, type: (int)message.MessageType);

            await BroadcastMessage(mess);
        }

        private async void ActiveServerPerformanceMeasured(object? sender, ValueEventArgs<(string CPU, string Memory)> e)
        {
            string cpu = e.NewValue.CPU;
            string memory = e.NewValue.Memory;

            string mess = MessageFormatter.PcUsage(cpu, memory);
            await BroadcastMessage(mess);
        }

        private async void ActiveServerStatusChange(object? sender, ValueEventArgs<ServerStatus> e)
        {
            if (sender is not IMinecraftServer mcServer)
                return;

            string message = MessageFormatter.StatusUpdate(e.NewValue, mcServer.OnlineFrom, mcServer.StorageSpace);
            await BroadcastMessage(message);
        }

        private async void ServerAdded(object? sender, ValueEventArgs<IMinecraftServer> e)
        {
            string name = e.NewValue.ServerName;
            string message = MessageFormatter.ServerAdded(name);

            await BroadcastMessage(message);
        }

    private async void ServerDeleted(object? sender, ValueEventArgs<IMinecraftServer> e)
        {
            string name = e.NewValue.ServerName;
            string message = MessageFormatter.ServerDeleted(name);

            await BroadcastMessage(message);
        }

        private async void ServerNameChanged(object? sender, ValueChangedEventArgs<string> e)
        {
            string oldName = e.OldValue;
            string newName = e.NewValue;

            string message = MessageFormatter.ServerNameChanged(oldName, newName);

            await BroadcastMessage(message);
        }

        private async void PermissionRemoved(object? sender, string e)
        {
            string code = e;

            string message = MessageFormatter.Logout();
            await BroadcastMessage(message, code);

            var sockets = GetAllSockets(code);

            RemoveSockets(code);

            foreach (var item in GetAllSockets(code))
            {
                _ = item.Close();
            }
        }

        #endregion


        #region Critical Part

        /// <summary>
        /// Registers the MCWebSocket to the list.
        /// </summary>
        /// <param name="socket">Socket to add.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RegisterSocket(MCWebSocket socket) =>
            Sockets.Add(socket);

        /// <summary>
        /// Removes a socket from the list.
        /// </summary>
        /// <param name="socket">Socket to remove.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RemoveSocket(MCWebSocket socket) =>
            Sockets.Remove(socket);


        /// <summary>
        /// Removes all sockets associated with the specified code.
        /// </summary>
        /// <param name="code">associated code to remove.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RemoveSockets(string code)
        {
            var sockets = GetAllSockets(code);

            foreach (var s in sockets)
                Sockets.Remove(s);
        }

        /// <summary>
        /// Gets all the sockets.
        /// </summary>
        /// <returns>All the registered sockets.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private IEnumerable<MCWebSocket> GetAllSockets() =>
            new List<MCWebSocket>(Sockets);

        /// <summary>
        /// Gets all the sockets which is registered by the specified code.
        /// </summary>
        /// <param name="code">the specified code.</param>
        /// <returns>All the sockets which is registered by the specified code.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private IEnumerable<MCWebSocket> GetAllSockets(string code) =>
            from s in Sockets where s.Code == code select s;

        #endregion

    }
}
