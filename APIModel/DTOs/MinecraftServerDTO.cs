using Shared.Model;

namespace APIModel.DTOs
{
    public class MinecraftServerDTO
    {
        public long Id { get; set; }
        public string ServerName { get; set; }
        public int Status { get; set; }
        public IEnumerable<ILogMessage> LogMessages { get; set; }
        public DateTime? OnlineFrom { get; set; }
        public int Port { get; }
        public ICollection<IMinecraftPlayer> Players { get; set; }
        public long StorageBytes { get; set; }

        public MinecraftServerDTO(IMinecraftServer server)
        {
            Id = server.Id;
            ServerName = server.ServerName;
            Status = (int)server.Status;
            LogMessages = server.Logs.TakeLast(50);
            OnlineFrom = server.OnlineFrom;
            Port = server.Port;
            Players = server.Players.Values.ToList();
            StorageBytes = server.StorageBytes;
        }
    }
}
