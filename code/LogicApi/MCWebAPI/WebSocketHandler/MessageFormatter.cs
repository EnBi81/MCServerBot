using Newtonsoft.Json;
using Application.Minecraft.MinecraftServers;
using SharedPublic.Model;
using static SharedPublic.Model.ILogMessage;
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
                server = server
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
                logs = (from logMessage in messages select logMessage).ToList()
            };

            return Serialize(log);
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
