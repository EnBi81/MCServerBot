using Newtonsoft.Json;
using Application.Minecraft.MinecraftServers;
using Shared.Model;
using static Shared.Model.ILogMessage;
using MCWebAPI.Utils;

namespace MCWebAPI.WebSocketHandler
{
    public static class MessageFormatter
    {
        public static string Logout()
        {
            var logout = new { datatype = "logout"};
            return Serialize(logout);
        }



        public static string ServerNameChanged(long id, string newName)
        {
            var nameChange = new
            {
                datatype = "serverNameChange",
                serverId = id,
                newName,
            };

            return Serialize(nameChange);
        }

        public static string ServerDeleted(long id)
        {
            var deleted = new { datatype = "serverDeleted", serverId = id };
            return Serialize(deleted);
        }

        public static string ServerAdded(long id, IMinecraftServer server)
        {
            var added = new { 
                datatype = "serverAdded", 
                serverId = id, 
                server = server.ToDTO()
            };
            return Serialize(added);
        }

        public static string Log(long id, string message, int type)
        {
            return Log(id, new LogMessage(message, (LogMessageType)type));
        }

        public static string Log(long id, LogMessage message)
        {
            return Log(id, new List<LogMessage>() { message });
        }

        public static string Log(long id, IEnumerable<LogMessage> messages)
        {
            var log = new
            {
                datatype = "log",
                serverId = id,
                logs = (from logMessage in messages select logMessage.ToDTO()).ToList()
            };

            return Serialize(log);
        }

        public static string PlayerJoin(long id, IMinecraftPlayer player)
        {
            var playerJoin = new
            {
                datatype = "playerJoin",
                serverId = id,
                player = player.ToDTO()
            };
            return Serialize(playerJoin);
        }

        public static string PlayerLeft(long id, IMinecraftPlayer player)
        {
            var playerLeft = new 
            {
                datatype = "playerLeft",
                serverId = id, 
                player = player.ToDTO() 
            };
            return Serialize(playerLeft);
        }

        public static string PcUsage(long serverId, string cpuPerc, string memory)
        {
            var pcUsage = new { datatype = "pcUsage", serverId, cpu = cpuPerc, memory = memory };
            return Serialize(pcUsage);
        }

        public static string StatusUpdate(long serverId, ServerStatus status, DateTime? onlineFrom, string storage)
        {
            string stringStatus = status switch
            {
                ServerStatus.Starting => "starting",
                ServerStatus.ShuttingDown => "shutting-down",
                ServerStatus.Online => "online",
                ServerStatus.Offline => "offline",
                _ => "offline"
            };

            var statusResponse = new { 
                datatype = "status",
                serverId,
                status = stringStatus, 
                onlineFrom = onlineFrom?.DateToString(),
                storage = storage
            };
            return Serialize(statusResponse);
        }

        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static string DateToString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "T");
        }
    }
}
