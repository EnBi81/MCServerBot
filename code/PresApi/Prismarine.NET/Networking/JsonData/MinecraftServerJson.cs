using Prismarine.NET.Model;
using Prismarine.NET.Model.Enums;

namespace Prismarine.NET.Networking.JsonData
{
    /// <summary>
    /// Represents a Minecraft server's JSON data.
    /// </summary>
    internal class MinecraftServerJson
    {
        /// <summary>
        /// Id of the server
        /// </summary>
        public long Id { get; init; }
        /// <summary>
        /// Name of the server
        /// </summary>
        public string? ServerName { get; set; }
        /// <summary>
        /// Status of the server
        /// </summary>
        public ServerStatus Status { get; set; }
        /// <summary>
        /// If the server is running
        /// </summary>
        public bool IsRunning { get; set; }
        /// <summary>
        /// Log messages
        /// </summary>
        public ICollection<LogMessage>? Logs { get; init; }
        /// <summary>
        /// Online from this time
        /// </summary>
        public DateTime? OnlineFrom { get; set; }
        /// <summary>
        /// Number of bytes the server occupies on the disk
        /// </summary>
        public long StorageBytes { get; set; }
        /// <summary>
        /// Minecraft version of the server
        /// </summary>
        public MinecraftVersion? MCVersion { get; set; }
    }
}
