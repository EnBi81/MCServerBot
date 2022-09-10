using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebServer.MinecraftServer
{
    /// <summary>
    /// Represents the status of a server
    /// </summary>
    public enum ServerStatus
    {
        Online,
        Offline,
        Starting,
        ShuttingDown
    }

    /// <summary>
    /// Extension class for ServerStatus enum
    /// </summary>
    internal static class ServerStatusExtensions
    {
        /// <summary>
        /// Converts the enum variable to string
        /// </summary>
        /// <param name="status"></param>
        /// <returns>the string representative of the enum value</returns>
        public static string DisplayString(this ServerStatus status)
        {
            if (status == ServerStatus.Online)
                return $"Server Online on {Hamachi.HamachiClient.Address}:{ServerPark.Keklepcso.Port}";
            if (status == ServerStatus.Offline)
                return "Server Offline";
            if (status == ServerStatus.Starting)
                return "Server Starting";
            if (status == ServerStatus.ShuttingDown)
                return "Server Shutting Down";

            return "";
        }
    }
}
