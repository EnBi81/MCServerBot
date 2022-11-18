using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace MCWebAPI.SignalR.Hubs
{
    [SignalRHub("/hubs/serverpark")]
    public class ServerParkHub : Hub
    {


        [SignalRListener("Receive")]
        public ServerParkHub() { }


        [SignalRMethod("Hi", SignalRSwaggerGen.Enums.Operation.Get)]
        public void Hi(string text, double other, float num, int num2, long n)
        {
            Clients.All.SendAsync("Receive", text, other, num, num2, n);
        }
        [SignalRMethod("Hello", SignalRSwaggerGen.Enums.Operation.Get)]
        public void Hello(string text, int[] arr)
        {
            Clients.All.SendAsync("Receive", text, arr);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
