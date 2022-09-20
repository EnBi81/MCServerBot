using Newtonsoft.Json;
using System.Collections.Generic;
using MCWebServer.MinecraftServer.Enums;
using System.Net.NetworkInformation;

namespace MCWebServer.WebSocketHandler
{
    public static class MessageFormatter
    {

        public static string Logout()
        {
            var logout = new { datatype = "logout"};
            return Serialize(logout);
        }



        public static string ServerNameChanged(string oldName, string newName)
        {
            var nameChange = new
            {
                datatype = "serverNameChange",
                oldName,
                newName
            };

            return Serialize(nameChange);
        }

        public static string ServerDeleted(string name)
        {
            var deleted = new { datatype = "serverDeleted", name };
            return Serialize(deleted);
        }

        public static string ServerAdded(string name)
        {
            var added = new { datatype = "serverAdded", name };
            return Serialize(added);
        }

        public static string ActiveServerChange(string newServer)
        {
            var active = new { datatype = "activeServerChange", newActive = newServer }
            return Serialize(active);
        }


        public static string Log(string message, int type)
        {
            return Log(new LogMessage(message, (LogMessage.LogMessageType)type));
        }

        public static string Log(LogMessage message)
        {
            return Log(new List<LogMessage>() { message });
        }

        public static string Log(IEnumerable<LogMessage> messages)
        {
            var log = new
            {
                datatype = "log",
                logs = (from logMessage in messages
                       select new
                       {
                           message = logMessage.Message,
                           type = (int)logMessage.MessageType
                       }).ToList()
            };

            return Serialize(log);
        }

        public static string PlayerJoin(string username, DateTime onlineFrom, TimeSpan pastUptime)
        {
            var playerJoin = new
            {
                datatype = "playerJoin",
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

        public static string PlayerLeft(string username)
        {
            var playerLeft = new { datatype = "playerLeft", username = username };
            return Serialize(playerLeft);
        }

        public static string PcUsage(string cpuPerc, string memory)
        {
            var pcUsage = new { datatype = "pcUsage", cpu = cpuPerc, memory = memory };
            return Serialize(pcUsage);
        }

        public static string StatusUpdate(ServerStatus status, DateTime? onlineFrom, string storage)
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
