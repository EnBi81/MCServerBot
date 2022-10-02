using Application.MinecraftServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MinecraftServer
{
    public interface IMinecraftServerSimplified
    {
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
        /// Gets the port associated with the server.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Phisical storage space on the disk of the server in BYTES.
        /// </summary>
        public long StorageBytes { get; }
    }
}
