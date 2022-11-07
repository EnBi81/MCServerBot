using Newtonsoft.Json;
using Application.Minecraft.MinecraftServers;
using Shared.Model;
using static Shared.Model.ILogMessage;

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
                id,
                newName
            };

            return Serialize(nameChange);
        }

        public static string ServerDeleted(long id)
        {
            var deleted = new { datatype = "serverDeleted", id };
            return Serialize(deleted);
        }

        public static string ServerAdded(long id)
        {
            var added = new { datatype = "serverAdded", id };
            return Serialize(added);
        }

        public static string Log(string server, string message, int type)
        {
            return Log(server, new LogMessage(message, (LogMessageType)type));
        }

        public static string Log(string server, LogMessage message)
        {
            return Log(server, new List<LogMessage>() { message });
        }

        public static string Log(string server, IEnumerable<LogMessage> messages)
        {
            var log = new
            {
                datatype = "log",
                server,
                logs = (from logMessage in messages
                       select new
                       {
                           message = logMessage.Message,
                           type = (int)logMessage.MessageType
                       }).ToList()
            };

            return Serialize(log);
        }

        public static string PlayerJoin(string server, string username, DateTime onlineFrom, TimeSpan pastUptime)
        {
            var playerJoin = new
            {
                datatype = "playerJoin",
                server,
                username = username,
                onlineFrom = onlineFrom.DateToString(),
                pastUptime = new {
                    h = pastUptime.Hours, 
                    m = pastUptime.Minutes,
                    s = pastUptime.Seconds
                }
            };
            return Serialize(playerJoin);
        }

        public static string PlayerLeft(string server, string username)
        {
            var playerLeft = new { datatype = "playerLeft", server, username = username };
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
