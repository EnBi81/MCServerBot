using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebServer.MinecraftServer
{
    internal enum ServerStatus
    {
        Online,
        Offline,
        Starting,
        ShuttingDown
    }

    internal static class ServerStatusExtensions
    {
        public static string DisplayString(this ServerStatus status)
        {
            if (status == ServerStatus.Online)
                return $"Server Online on {Hamachi.HamachiClient.GetStatus().Address}:{ServerPark.Keklepcso.Port}";
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
