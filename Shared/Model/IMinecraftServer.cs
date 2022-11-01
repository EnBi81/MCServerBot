﻿
namespace Shared.Model
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
        public string ServerName { get; set; }

        /// <summary>
        /// Gets the status of the server.
        /// </summary>
        public ServerStatus Status { get; }

        /// <summary>
        /// Gets if the server process is running.
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        /// All of the log messages the server or the users wrote.
        /// </summary>
        public List<ILogMessage> Logs { get; }

        /// <summary>
        /// The time of the server when it became online, or null if the server is offline
        /// </summary>
        public DateTime? OnlineFrom { get; }

        /// <summary>
        /// Access to the properties file of the server.
        /// </summary>
        public IMinecraftServerProperties Properties { get; }

        /// <summary>
        /// Gets the port associated with the server.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets all the currently online players.
        /// </summary>
        public List<IMinecraftPlayer> OnlinePlayers => (from player in Players.Values where player.OnlineFrom.HasValue select player).ToList();

        /// <summary>
        /// Holding all the players who have played in the server, from the beginning of the current runtime.
        /// </summary>
        public Dictionary<string, IMinecraftPlayer> Players { get; }

        /// <summary>
        /// Physical storage space on the disk of the server.
        /// </summary>
        public string StorageSpace { get; }

        /// <summary>
        /// Phisical storage space on the disk of the server in BYTES.
        /// </summary>
        public long StorageBytes { get; }


        /// <summary>
        /// Starts the server
        /// </summary>
        /// <param name="user">username of the user who initiates the start of the server.</param>
        public void Start(string user = "Admin");

        /// <summary>
        /// Writes a command to the minecraft serves based on the state of the server.
        /// </summary>
        /// <param name="command">command to send to the minecraft server.</param>
        /// <param name="user">username of the user who sends the command.</param>
        public void WriteCommand(string command, string user = "Admin");

        /// <summary>
        /// Shuts down the minecraft server if it is online.
        /// </summary>
        /// <param name="user">username of the user who initiates the start of the server.</param>
        public void Shutdown(string user = "Admin");

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
        public event EventHandler<IMinecraftPlayer> PlayerJoined;
        /// <summary>
        /// Fired when a player has left the server.
        /// </summary>
        public event EventHandler<IMinecraftPlayer> PlayerLeft;
        /// <summary>
        /// Fired when performance has been measured of the minecraft server process.
        /// </summary>
        public event EventHandler<(string CPU, string Memory)> PerformanceMeasured;
        /// <summary>
        /// Fired when the server has changed its name.
        /// </summary>
        public event EventHandler<string> NameChanged;
    }
}
