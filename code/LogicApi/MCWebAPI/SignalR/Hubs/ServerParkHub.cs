using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace MCWebAPI.SignalR.Hubs
{
    [SignalRHub("/hubs/serverpark")]
    public class ServerParkHub : Hub
    {
        public const string PlayerLeft = "PlayerLeft";
        public const string PlayerJoined = "PlayerJoined";
        public const string LogReceived = "LogReceived";
        public const string PerformanceMeasured = "PerformanceMeasured";
        public const string StatusChange = "StatusChange";
        public const string ServerAdded = "ServerAdded";
        public const string ServerDeleted = "ServerDeleted";
        public const string ServerModified = "ServerModified";
        

        [SignalRListener(PlayerLeft)]
        [SignalRListener(PlayerJoined)]
        [SignalRListener(LogReceived)]
        [SignalRListener(PerformanceMeasured)]
        [SignalRListener(StatusChange)]
        [SignalRListener(ServerAdded)]
        [SignalRListener(ServerDeleted)]
        [SignalRListener(ServerModified)]
        public ServerParkHub() { }
        
    }
}
