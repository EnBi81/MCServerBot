using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    /// <summary>
    /// Represents the status of a server
    /// </summary>
    public enum ServerStatus
    {
        /// <summary>
        /// Server process is not running, the server is offline.
        /// </summary>
        Offline,

        /// <summary>
        /// Server process has been started, it is not ready though.
        /// </summary>
        Starting,

        /// <summary>
        /// Server fully functions, it's online
        /// </summary>
        Online,

        /// <summary>
        /// Server process is shutting down, expect to go offline in some seconds.
        /// </summary>
        ShuttingDown
    }

    /// <summary>
    /// Extension class for ServerStatus enum
    /// </summary>
    public static class ServerStatusExtensions
    {
        /// <summary>
        /// Converts the enum variable to string
        /// </summary>
        /// <param name="status"></param>
        /// <returns>the string representative of the enum value</returns>
        public static string DisplayString(this ServerStatus status)
        {
            return status switch
            {
                ServerStatus.Offline => "Offline",
                ServerStatus.Starting => "Starting",
                ServerStatus.Online => "Online",
                ServerStatus.ShuttingDown => "Shutting Down",
                _ => "NOT IMPLEMENTED SERVER STATUS"
            };
        }
    }
}
