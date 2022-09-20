using MCWebServer.Log;
using MCWebServer.PermissionControll;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using MCWebServer.MinecraftServer;
using MCWebServer.MinecraftServer.EventHandlers;
using MCWebServer.MinecraftServer.Enums;

namespace MCWebServer.WebSocketHandler
{
    /// <summary>
    /// SocketPool stores and takes care of all the incoming and established websockets.
    /// </summary>
    public class SocketPoolv2
    {
        /// <summary>
        /// Single SocketPool reference.
        /// </summary>
        public static SocketPoolv2 SocketPool { get; private set; }

        /// <summary>
        /// Initializes the SocketPool
        /// </summary>
        public static void InitializePool()
        {
            SocketPool = new SocketPoolv2();
        }


        /// <summary>
        /// Store all the sockets.
        /// </summary>
        private static ICollection<MCWebSocket> Sockets { get; } = new List<MCWebSocket>();

        /// <summary>
        /// Initializes the SocketPool.
        /// </summary>
        private SocketPoolv2()
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
        public async Task BroadcastMessage(string message, string code = null)
        {
            List<Task> tasks = new();

            var socketsToBroadcast = code == null ? GetAllSockets() : GetAllSockets(code);

            foreach (var socket in socketsToBroadcast)
                tasks.Add(socket.SendMessage(message));

            foreach (var task in tasks)
                await task;
        }

        #endregion


        #region Listeneres
        private void ActiveServerChange(object sender, ValueEventArgs<IMinecraftServer> e)
        {
            throw new NotImplementedException();
        }

        private void ActiveServerPlayerLeft(object sender, ValueEventArgs<MinecraftPlayer> e)
        {
            throw new NotImplementedException();
        }

        private void ActiveServerPlayerJoined(object sender, ValueEventArgs<MinecraftPlayer> e)
        {
            throw new NotImplementedException();
        }

        private void ActiveServerLogReceived(object sender, ValueEventArgs<LogMessage> e)
        {
            throw new NotImplementedException();
        }

        private void ActiveServerPerformanceMeasured(object sender, ValueEventArgs<(string CPU, string Memory)> e)
        {
            throw new NotImplementedException();
        }

        private void ActiveServerStatusChange(object sender, ValueEventArgs<ServerStatus> e)
        {
            throw new NotImplementedException();
        }

        private void ServerAdded(object sender, ValueEventArgs<IMinecraftServer> e)
        {
            throw new NotImplementedException();
        }

        private void ServerDeleted(object sender, ValueEventArgs<IMinecraftServer> e)
        {
            throw new NotImplementedException();
        }

        private void ServerNameChanged(object sender, ValueChangedEventArgs<string> e)
        {
            throw new NotImplementedException();
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
