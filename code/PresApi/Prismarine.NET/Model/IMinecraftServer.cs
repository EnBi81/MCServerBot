using Prismarine.NET.Model.Enums;

namespace Prismarine.NET.Model
{
    public interface IMinecraftServer
    {
        /// <summary>
        /// Id number, unique for a minecraft server.
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// Gets the status code of the server.
        /// </summary>
        ServerStatus Status { get; }
        
        /// <summary>
        /// Gets if the server process is running.
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// Gets the last 50 log messages from the server.
        /// </summary>
        ICollection<LogMessage> Logs { get; }

        /// <summary>
        /// The time of the server when it became online, or null if the server is offline
        /// </summary>
        DateTime? OnlineFrom { get; }

        /// <summary>
        /// Phisical storage space on the disk of the server in BYTES.
        /// </summary>
        long StorageBytes { get; }

        /// <summary>
        /// Gets the minecraft version of the server.
        /// </summary>
        MinecraftVersion MCVersion { get; }

        /// <summary>
        /// Gets the server icon.
        /// </summary>
        string ServerIcon { get; }


        /// <summary>
        /// Refreshes the local data from the server.
        /// </summary>
        /// <returns></returns>
        Task Refresh();

        Task Modify();

        Task Delete();

        Task WriteCommand();

        Task Toggle();
    }
}
