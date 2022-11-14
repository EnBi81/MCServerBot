using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace MCWebAPI.SignalR.Hubs
{
    [SignalRHub("/hubs/serverpark")]
    public class ServerParkHub : Hub
    {
        [SignalRMethod("Receive", SignalRSwaggerGen.Enums.Operation.Get)]
        public void Hi()
        {
            Clients.All.SendAsync("Receive", "heyy");
        }
    }
}
