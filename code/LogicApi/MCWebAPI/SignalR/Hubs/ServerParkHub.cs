using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace MCWebAPI.SignalR.Hubs
{
    [SignalRHub("/hubs/serverpark")]
    public class ServerParkHub : Hub
    {

        [SignalRListener("Receive")]
        public ServerParkHub() { }


        [SignalRMethod("Receive", SignalRSwaggerGen.Enums.Operation.Get)]
        public void Hi(string text)
        {
            Clients.All.SendAsync("Receive", "heyy");
        }
        [SignalRMethod("Hello", SignalRSwaggerGen.Enums.Operation.Get)]
        public void Hello(string text)
        {
            Console.WriteLine(text);
        }
    }
}
