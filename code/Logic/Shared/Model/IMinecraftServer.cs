using Application.Minecraft.Versions;
using SharedPublic.DTOs;
using SharedPublic.Exceptions;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SharedPublic.Model
{
    /// <summary>
    /// Interface representing a single Minecraft Server.
    /// </summary>
    public interface IMinecraftServer
    {
        /// <summary>
        /// Minimum allowed length of the ServerName
        /// </summary>
        public const int NAME_MIN_LENGTH = 4;
        /// <summary>
        /// Maximum allowed length of the ServerName
        /// </summary>
        public const int NAME_MAX_LENGTH = 35;

        /// <summary>
        /// Id number, unique for a minecraft server.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets or sets the name of the server. Raises a <see cref="NameChanged"/> event.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        /// Gets the status code of the server.
        /// </summary>
        public ServerStatus StatusCode { get; }

        public string StatusMessage => StatusCode.DisplayString();

        /// <summary>
        /// Gets if the server process is running.
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        /// Gets the last 50 log messages from the server.
        /// </summary>
        public ICollection<ILogMessage> Logs { get; }

        /// <summary>
        /// The time of the server when it became online, or null if the server is offline
        /// </summary>
        public DateTime? OnlineFrom { get; }

        /// <summary>
        /// Access to the properties file of the server.
        /// </summary>
        [JsonIgnore]
        public IMinecraftServerProperties Properties { get; }

        /// <summary>
        /// Gets the port associated with the server.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the players who have been connected to the server in the current session
        /// </summary>
        [JsonIgnore]
        public IDictionary<string, IPlayerFull> PlayersFull { get; }

        /// <summary>
        /// Gets the players who have been connected to the server in the current session
        /// </summary>
        public IEnumerable<IPlayerSimple> Players { get; }

        /// <summary>
        /// Physical storage space on the disk of the server.
        /// </summary>
        public string StorageSpace { get; }

        /// <summary>
        /// Phisical storage space on the disk of the server in BYTES.
        /// </summary>
        public long StorageBytes { get; }

        /// <summary>
        /// Gets the minecraft version of the server.
        /// </summary>
        public IMinecraftVersion MCVersion { get; }

        /// <summary>
        /// Gets and sets the server version of the server.
        /// </summary>
        [JsonIgnore]
        public string? ServerIcon { get; }


        /// <summary>
        /// Starts the server
        /// </summary>
        /// <param name="user">username of the user who initiates the start of the server.</param>
        /// <exception cref="MinecraftServerException">If the server is not Offline.</exception>
        public Task Start(UserEventData data = default);

        /// <summary>
        /// Writes a command to the minecraft serves based on the state of the server.
        /// </summary>
        /// <param name="command">command to send to the minecraft server.</param>
        /// <param name="data">user data of the user who sends the command.</param>
        /// <exception cref="MinecraftServerException">If the server is not Online.</exception>
        public Task<CommandResponse> WriteCommand(string? command, UserEventData data = default);

        /// <summary>
        /// Shuts down the minecraft server if it is online.
        /// </summary>
        /// <exception cref="MinecraftServerException">If the server is not Online.</exception>
        public Task Shutdown(UserEventData data = default);

        /// <summary>
        /// Backs up the server and saves it with the given name.
        /// </summary>
        /// <param name="name">name of the backup</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task Backup(BackupDto backup, UserEventData data = default);

        /// <summary>
        /// Restores the server from the given backup.
        /// </summary>
        /// <param name="backup">the backup to restore</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task Restore(IBackup backup, UserEventData data = default);

        /// <summary>
        /// Deletes the server.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task DeleteAsync(UserEventData data = default);

        /// <summary>
        /// Modifies the server.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ModifyAsync(ModifyServerDto dto, UserEventData user);

        /// <summary>
        /// Fired when the server has changed status.
        /// </summary>
        public event EventHandler<ServerStatus> StatusChange;
        /// <summary>
        /// Fired when a log message received.
        /// </summary>
        public event EventHandler<ILogMessage> LogReceived;
        /// <summary>
        /// Fired when a player has joined to the server.
        /// </summary>
        public event EventHandler<IPlayerSimple> PlayerJoined;
        /// <summary>
        /// Fired when a player has left the server.
        /// </summary>
        public event EventHandler<IPlayerSimple> PlayerLeft;
        /// <summary>
        /// Fired when performance has been measured of the minecraft server process. CPU is in percentage, Memory in Bytes.
        /// </summary>
        public event EventHandler<(double CPU, long Memory)> PerformanceMeasured;
        /// <summary>
        /// Fired when the server has changed its name.
        /// </summary>
        public event EventHandler<string> NameChanged;
        /// <summary>
        /// Fired when the server's storage has been measured. Unit is in bytes.
        /// </summary>
        public event EventHandler<long> StorageMeasured;
        /// <summary>
        /// Fired when the server's version has been changed.
        /// </summary>
        public event EventHandler<IMinecraftVersion> VersionChanged;
        /// <summary>
        /// Fired when the server is deleted.
        /// </summary>
        public event EventHandler<IMinecraftServer> Deleted;
    }
}
