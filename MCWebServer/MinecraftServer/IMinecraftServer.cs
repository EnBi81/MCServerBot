using MCWebServer.MinecraftServer.Enums;
using System.Collections.Generic;

namespace MCWebServer.MinecraftServer
{
    public interface IMinecraftServer
    {
        public const int NAME_MIN_LENGTH = 4;
        public const int NAME_MAX_LENGTH = 35;

        public string ServerName { get; set; }
        public int Port => int.Parse(Properties["server-port"]);
        public List<LogMessage> Logs { get; }
        public ServerStatus Status { get; }
        public bool IsRunning { get; }
        public DateTime? OnlineFrom { get; }
        public MinecraftServerProperties Properties { get; }
        public List<MinecraftPlayer> OnlinePlayers => (from player in Players.Values where player.OnlineFrom.HasValue select player).ToList();
        public Dictionary<string, MinecraftPlayer> Players { get; }
        public string StorageSpace { get; }


        public void Start(string user = "Admin");
        public void WriteCommand(string command, string user = "Admin");
        public void Shutdown(string user = "Admin");

        public event EventHandler<ServerStatus> StatusChange;
        public event EventHandler<LogMessage> LogReceived;
        public event EventHandler<MinecraftPlayer> PlayerJoined;
        public event EventHandler<MinecraftPlayer> PlayerLeft;
        public event EventHandler<(string CPU, string Memory)> PerformanceMeasured;
    }
}
