using APIModel.DTOs;
using SharedPublic.DTOs;
using SharedPublic.EventHandlers;
using SharedPublic.Exceptions;

namespace SharedPublic.Model
{
    /// <summary>
    /// A cozy place to manage the minecraft servers
    /// </summary>
    public interface IServerPark
    {
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        /// <returns></returns>
        public Task InitializeAsync();


        /// <summary>
        /// Online minecraft server (only one can be online at a time)
        /// </summary>
        public IMinecraftServer? ActiveServer { get; }

        /// <summary>
        /// Readonly collection of the minecraft servers
        /// </summary>
        public IReadOnlyDictionary<long, IMinecraftServer> MCServers { get; }

        /// <summary>
        /// Version collection and manager
        /// </summary>
        public IMinecraftVersionCollection MinecraftVersionCollection { get; }

        /// <summary>
        /// Backup manager
        /// </summary>
        public IBackupManager BackupManager { get; }

        


        /// <summary>
        /// Gets a server by id.
        /// </summary>
        /// <param name="id">id of the server</param>
        /// <returns></returns>
        /// <exception cref="ServerParkException">If the id is not registered.</exception>
        public IMinecraftServer GetServer(long id);

        /// <summary>
        /// Start the active server.
        /// </summary>
        /// <param name="id">id of the minecraft server to start</param>
        /// <param name="user">user who initiated the start</param>
        /// <exception cref="ServerParkException">If the max storage has been reached.</exception>
        /// <exception cref="ServerParkException">If the id is not registered in the serverpark</exception>
        /// <exception cref="ServerParkException">If another server is running</exception>
        public Task StartServer(long id, UserEventData user = default);

        /// <summary>
        /// Stop the active server.
        /// </summary>
        /// <param name="user">user who initiated the stop</param>
        /// <exception cref="ServerParkException">If there is no running server.</exception>
        public Task StopActiveServer(UserEventData user = default);


        /// <summary>
        /// Toggles a server, e.g. it starts if it's offline, and stops if it's online.
        /// For Exceptions please see <see cref="StartServer(long, UserEventData)"/> and <see cref="StopActiveServer(UserEventData)"/>
        /// </summary>
        /// <param name="id">id of the server to toggle</param>
        /// <param name="user">user who initiated this action</param>
        public Task ToggleServer(long id, UserEventData user = default);


        /// <summary>
        /// Creates a new server folder by copying the empty folder to the servers folder.
        /// </summary>
        /// <param name="dto">Information to create the server</param>
        /// <exception cref="MinecraftServerArgumentException">If the name is null or white space, or the name of the server is not in <see cref="IMinecraftServer.NAME_MIN_LENGTH"/> and <see cref="IMinecraftServer.NAME_MAX_LENGTH"/></exception>
        /// <exception cref="ServerParkException">If the name already exists.</exception>
        /// <exception cref="ServerParkException">If the max storage has been reached.</exception>
        public Task<IMinecraftServer> CreateServer(ServerCreationDto dto, UserEventData user = default);

        /// <summary>
        /// Changes an already existing minecraft server's name if it is not running
        /// </summary>
        /// <param name="oldName">Name of the server to change</param>
        /// <param name="newName">New name of the server</param>
        /// <exception cref="MinecraftServerArgumentException">If the name is null or white space, or the name of the server is not in <see cref="IMinecraftServer.NAME_MIN_LENGTH"/> and <see cref="IMinecraftServer.NAME_MAX_LENGTH"/></exception>
        /// <exception cref="ServerParkException">If the id is not registered</exception>
        /// <exception cref="ServerParkException">If the server is running</exception>
        /// <exception cref="ServerParkException">If the name already exists</exception>
        public Task<IMinecraftServer> ModifyServer(long id, ModifyServerDto dto, UserEventData user = default);

        /// <summary>
        /// Deletes a server by moving to the <see cref="DeletedServersFolder"/>.
        /// </summary>
        /// <param name="name">Server to be moved.</param>
        /// <exception cref="Exception">If the server does not exist, or it's running.</exception>
        public Task<IMinecraftServer> DeleteServer(long id, UserEventData user = default);


        

        /// <summary>
        /// Event fired when the active server's status has changed.
        /// </summary>
        public event EventHandler<ServerValueEventArgs<ServerStatus>> ActiveServerStatusChange;

        /// <summary>
        /// Event fired when a log message has been received from the active server.
        /// </summary>
        public event EventHandler<ServerValueEventArgs<ILogMessage>> ActiveServerLogReceived;

        /// <summary>
        /// Event fired when a player joins the active server.
        /// </summary>
        public event EventHandler<ServerValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerJoined;

        /// <summary>
        /// Event fired when a player leaves the active server.
        /// </summary>
        public event EventHandler<ServerValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerLeft;

        /// <summary>
        /// Event fired when a performance measurement data has been received. CPU in percentage, Memory in bytes.
        /// </summary>
        public event EventHandler<ServerValueEventArgs<(double CPU, long Memory)>> ActiveServerPerformanceMeasured;

        /// <summary>
        /// Event fired when a server has been modified.
        /// </summary>
        public event EventHandler<ServerValueEventArgs<ModifyServerDto>> ServerModified;

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
