using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebServer.MinecraftServer
{
    internal class ServerPark
    {
        public static MinecraftServer Keklepcso { get; } 
            = new MinecraftServer(
                Config.Config.Instance.MinecraftServerName,
                Hamachi.HamachiClient.GetStatus().Address,
                Config.Config.Instance.MinecraftServerFile,
                Config.Config.Instance.MinecraftServerProperties);
    }
}
