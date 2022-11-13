using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace MCWebAPI.SignalR.Hubs
{
    [SignalRHub("/testroute/serverpark")]
    public class ServerParkHub : Hub
    {
        [SignalRMethod("Hello", SignalRSwaggerGen.Enums.Operation.Get)]
        public void Hi(string text)
        {
            Clients.All.SendAsync("Receive", text);
        }
    }
}
