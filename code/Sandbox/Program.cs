using CoreRCON;
using System.Net;

namespace Sandbox
{
    public class SandBoxClass
    {
        static async Task Main(string[] args)
        {
            // https://www.nuget.org/packages/CoreRCON

            // Connect to a server
            var rcon = new RCON(IPAddress.Parse("127.0.0.1"), 25575, "12345678");
            await rcon.ConnectAsync();

            Console.WriteLine("Connected to server");
            
            string respnose = await rcon.SendCommandAsync("data get entity @e[limit=1]");
            
            Console.WriteLine($"Response: " + respnose);

            rcon.Dispose();
        }
    }
}

