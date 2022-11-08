using APIModel.DTOs;
using Shared.Model;

namespace MCWebAPI.Utils
{
    public static class DTOConverter
    {
        public static MinecraftServerDTO ToDTO(this IMinecraftServer server) => new()
        {
            Id = server.Id,
            ServerName = server.ServerName,
            OnlineFrom = server.OnlineFrom,
            Status = (int)server.Status,
            StorageBytes = server.StorageBytes,
            LogMessages = from log in server.Logs.TakeLast(50) select log.ToDTO(),
            Players = from player in server.Players.Values select player.ToDTO(),
            Port = server.Port,
        };

        public static LogMessageDto ToDTO(this ILogMessage logMessage) => new()
        {
            Message = logMessage.Message,
            MessageType = (int)logMessage.MessageType,
        };

        public static MinecraftPlayerDTO ToDTO(this IMinecraftPlayer player) => new()
        {
            Username = player.Username,
            OnlineFrom = player.OnlineFrom,
            PastOnline = player.PastOnline,
        };
    }
}
