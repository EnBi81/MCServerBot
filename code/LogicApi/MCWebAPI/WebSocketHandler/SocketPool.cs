using Loggers;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using Shared.Model;
using Shared.EventHandlers;
using Application.Permissions;
using Shared.DTOs;
using Loggers.Loggers;

namespace MCWebAPI.WebSocketHandler
{
    /// <summary>
    /// SocketPool stores and takes care of all the incoming and established websockets.
    /// </summary>
    public class SocketPool
    {

        /// <summary>
        /// Store all the sockets.
        /// </summary>
        private ICollection<MCWebSocket> Sockets { get; } = new List<MCWebSocket>();



        private readonly IServerPark _serverPark;
        private readonly IPermissionLogic _permissionLogic;
        private readonly WebApiLogger _logger;


        /// <summary>
        /// Initializes the SocketPool.
        /// </summary>
        public SocketPool(IServerPark serverPark, IPermissionLogic permissionLogic, WebApiLogger logger)
        {
            _serverPark = serverPark;
            _permissionLogic = permissionLogic;
            _logger = logger;

            _logger.Log("socket-pool", "Socket Pool Initalizing");
            SetupListeners();
            _logger.Log("socket-pool", "Socket Pool Initalized");
        }

        #region Base methods

        /// <summary>
        /// Sets up the listeners for the ServerPark.
        /// </summary>
        private void SetupListeners()
        {
            _logger.Log("socket-pool", "Setting up listeners");
            
            _serverPark.ActiveServerPlayerLeft += ActiveServerPlayerLeft;
            _serverPark.ActiveServerPlayerJoined += ActiveServerPlayerJoined;
            _serverPark.ActiveServerLogReceived += ActiveServerLogReceived;
            _serverPark.ActiveServerPerformanceMeasured += ActiveServerPerformanceMeasured;
            _serverPark.ActiveServerStatusChange += ActiveServerStatusChange;
            
            _serverPark.ServerAdded += ServerAdded;
            _serverPark.ServerDeleted += ServerDeleted;
            _serverPark.ServerModified += ServerNameChanged;

            _permissionLogic.PermissionRevoked += PermissionRevoked;

            _logger.Log("socket-pool", "Listeners have been set up");
        }

        


        /// <summary>
        /// Adds a socket to the SocketPool
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <param name="socket">captured websocket.</param>
        /// <returns></returns>
        public async Task AddSocket(ulong id, WebSocket socket)
        {
            var user = await _permissionLogic.GetUser(id);

            _logger.Log("socket-pool", "New socket received from " + user!.Username);
            MCWebSocket socketHandler = new (socket, user, _logger);

            RegisterSocket(socketHandler);

            await socketHandler.Initialize();
        }


        /// <summary>
        /// Broadcast a message to the connected sockets.
        /// </summary>
        /// <param name="message">Message to broadcast.</param>
        /// <param name="id">user's id to broadcast the message to. If not specified, it broadcasts the message to all the sockets.</param>
        /// <returns></returns>
        public async Task BroadcastMessage(string message, ulong? id = null)
        {
            var socketsToBroadcast = id == null ? GetAllSockets() : GetAllSockets(id ?? 0);

            foreach (var socket in socketsToBroadcast)
            {
                if(!socket.IsOpen)
                {
                    RemoveSocket(socket);
                    continue;
                }

                await socket.SendMessage(message);
            }
        }

        #endregion


        #region Listeners

        private async void ActiveServerPlayerLeft(object? sender, ServerValueEventArgs<IMinecraftPlayer> e)
        {
            if (sender is not IMinecraftServer server)
                return;

            IMinecraftPlayer player = e.NewValue;
            string mess = MessageFormatter.PlayerLeft(server.Id, player);

            await BroadcastMessage(mess);
        }

        private async void ActiveServerPlayerJoined(object? sender, ServerValueEventArgs<IMinecraftPlayer> e)
        {
            if (sender is not IMinecraftServer server)
                return;

            IMinecraftPlayer player = e.NewValue;
            if (player.OnlineFrom is null)
                return;

            string mess = MessageFormatter.PlayerJoin(server.Id, player);

            await BroadcastMessage(mess);
        }

        private async void ActiveServerLogReceived(object? sender, ServerValueEventArgs<ILogMessage> e)
        {
            if (sender is not IMinecraftServer server)
                return;

            ILogMessage message = e.NewValue;
            string mess = MessageFormatter.Log(server.Id, message.Message, type: (int)message.MessageType);

            await BroadcastMessage(mess);
        }

        private async void ActiveServerPerformanceMeasured(object? sender, ServerValueEventArgs<(double CPU, long Memory)> e)
        {
            if (sender is not IMinecraftServer server)
                return;

            string cpu = e.NewValue.CPU.ToString("0.00") + " %";
            string memory = e.NewValue.Memory / (1024 * 1024) + " MB";

            string mess = MessageFormatter.PcUsage(server.Id, cpu, memory);
            await BroadcastMessage(mess);
        }

        private async void ActiveServerStatusChange(object? sender, ServerValueEventArgs<ServerStatus> e)
        {
            if (sender is not IMinecraftServer mcServer)
                return;

            string message = MessageFormatter.StatusUpdate(mcServer.Id, e.NewValue, mcServer.OnlineFrom, mcServer.StorageSpace);
            await BroadcastMessage(message);
        }

        private async void ServerAdded(object? sender, ValueEventArgs<IMinecraftServer> e)
        {
            var id = e.NewValue.Id;
            string message = MessageFormatter.ServerAdded(id, e.NewValue);

            await BroadcastMessage(message);
        }

        private async void ServerDeleted(object? sender, ValueEventArgs<IMinecraftServer> e)
        {
            var id = e.NewValue.Id;
            string message = MessageFormatter.ServerDeleted(id);

            await BroadcastMessage(message);
        }

        private async void ServerNameChanged(object? sender, ServerValueEventArgs<ModifyServerDto> e)
        {
            string? newName = e.NewValue.NewName;

            if (newName is null)
                return;

            string message = MessageFormatter.ServerNameChanged(e.Server.Id, newName);

            await BroadcastMessage(message);
        }

        private async void PermissionRevoked(object? sender, DataUser user)
        {
            ulong id = user.Id;

            string message = MessageFormatter.Logout();
            await BroadcastMessage(message, id);

            var sockets = GetAllSockets(id);

            RemoveSockets(id);

            foreach (var item in sockets)
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
        private void RemoveSockets(ulong id)
        {
            var sockets = GetAllSockets(id);

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
        /// Gets all the sockets which is registered by the specified id.
        /// </summary>
        /// <param name="id">the specified id.</param>
        /// <returns>All the sockets which is registered by the specified code.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private IEnumerable<MCWebSocket> GetAllSockets(ulong id) =>
            from s in Sockets where s.DiscordUser.Id == id select s;

        #endregion

    }
}
