using APIModel.DTOs;
using Application.Minecraft.Versions;
using Shared.DTOs;
using Shared.Model;
using System.Security.Cryptography.Xml;

namespace MCWebAPI.Utils
{
    public static class DTOConverter
    {
        public static MinecraftServerDTO ToDTO(this IMinecraftServer server) => new()
        {
            Id = server.Id,
            ServerName = server.ServerName,
            OnlineFrom = server.OnlineFrom,
            StatusCode = (int)server.StatusCode,
            StatusMessage = server.StatusMessage,
            StorageBytes = server.StorageBytes,
            LogMessages = from log in server.Logs.TakeLast(50) select log.ToDTO(),
            Players = from player in server.Players.Values select player.ToDTO(),
            Port = server.Port,
            Version = server.MCVersion.Version,
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

        public static ServerChangeableDto ToModelDto(this ModifyServerDto dto)
        {
            return new ServerChangeableDto
            {
                NewName = dto.NewName,
                Version = dto.Version
            };
        }

        public static ServerChangeableDto ToModelDto(this ServerCreationDto dto)
        {
            return new ServerChangeableDto
            {
                NewName = dto.NewName,
                Version = dto.Version
            };
        }

        public static MinecraftVersionDto ToDto(this IMinecraftVersion version)
        {
            return new MinecraftVersionDto
            {
                Name = version.Name,
                Version = version.Version,
                IsDownloaded = version.IsDownloaded,
                DownloadUrl = version.DownloadUrl,
                ReleaseDate = version.ReleaseDate,
            };
        }
    }
}
