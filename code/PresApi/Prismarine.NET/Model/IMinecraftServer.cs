using Prismarine.NET.Model.Enums;

namespace Prismarine.NET.Model
{
    public interface IMinecraftServer
    {
        /// <summary>
        /// Id number, unique for a minecraft server.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        /// Gets the status code of the server.
        /// </summary>
        public ServerStatus Status { get; }
        
        /// <summary>
        /// Gets if the server process is running.
        /// </summary>
        public bool IsRunning { get; }
        
        /// <summary>
        /// Gets the last 50 log messages from the server.
        /// </summary>
        public ICollection<LogMessage> Logs { get; }

        /// <summary>
        /// The time of the server when it became online, or null if the server is offline
        /// </summary>
        public DateTime? OnlineFrom { get; }

        /// <summary>
        /// Phisical storage space on the disk of the server in BYTES.
        /// </summary>
        public long StorageBytes { get; }

        /// <summary>
        /// Gets the minecraft version of the server.
        /// </summary>
        public MinecraftVersion MCVersion { get; }
    }
}
