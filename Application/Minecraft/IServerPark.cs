using Application.Minecraft.Enums;
using Application.Minecraft.EventHandlers;
using Application.Minecraft.MinecraftServers;

namespace Application.Minecraft
{
    public interface IServerPark
    {

        /// <summary>
        /// Online minecraft server (only one can be online at a time)
        /// </summary>
        public IMinecraftServer? ActiveServer { get; }

        /// <summary>
        /// List of all minecraft server instances
        /// </summary>
        public Dictionary<string, IMinecraftServer> MCServers { get; }


        /// <summary>
        /// Start the active server.
        /// </summary>
        /// <param name="username">username who initiates the start</param>
        public void StartServer(string serverName, string username);

        /// <summary>
        /// Stop the active server.
        /// </summary>
        /// <param name="username">username who initiates the stop</param>
        public void StopActiveServer(string username);


        /// <summary>
        /// Toggles a server, e.g. it starts if it's offline, and stops if it's online.
        /// </summary>
        /// <param name="serverName">server to toggle</param>
        /// <param name="username">username who initiated this action</param>
        public void ToggleServer(string serverName, string username = "Admin");


        /// <summary>
        /// Creates a new server folder by copying the empty folder to the servers folder.
        /// </summary>
        /// <param name="name">name of the new </param>
        /// <exception cref="Exception"></exception>
        public IMinecraftServer CreateServer(string name);

        /// <summary>
        /// Changes an already existing minecraft server's name if it is not running
        /// </summary>
        /// <param name="oldName">Name of the server to change</param>
        /// <param name="newName">New name of the server</param>
        /// <exception cref="Exception">If the name has invalid length</exception>
        /// <exception cref="Exception">If the new name is already taken</exception>
        /// <exception cref="Exception">If the server to change is running</exception>
        public void RenameServer(string oldName, string newName);

        /// <summary>
        /// Deletes a server by moving to the <see cref="DeletedServersFolder"/>.
        /// </summary>
        /// <param name="name">Server to be moved.</param>
        /// <exception cref="Exception">If the server does not exist, or it's running.</exception>
        public void DeleteServer(string name);



        /// <summary>
        /// Event fired when the active server has changed.
        /// </summary>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange;

        /// <summary>
        /// Event fired when the active server's status has changed.
        /// </summary>
        public event EventHandler<ValueEventArgs<ServerStatus>> ActiveServerStatusChange;

        /// <summary>
        /// Event fired when a log message has been received from the active server.
        /// </summary>
        public event EventHandler<ValueEventArgs<LogMessage>> ActiveServerLogReceived;

        /// <summary>
        /// Event fired when a player joins the active server.
        /// </summary>
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerJoined;

        /// <summary>
        /// Event fired when a player leaves the active server.
        /// </summary>
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerLeft;

        /// <summary>
        /// Event fired when a performance measurement data has been received.
        /// </summary>
        public event EventHandler<ValueEventArgs<(string CPU, string Memory)>> ActiveServerPerformanceMeasured;

        /// <summary>
        /// Event fired when a server's name is changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<string>> ServerNameChanged;

        /// <summary>
        /// Event fired when a server is added to the ServerPark.
        /// </summary>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded;

        /// <summary>
        /// Event fired when a server is deleted from the ServerPark.
        /// </summary>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted;
    }
}
