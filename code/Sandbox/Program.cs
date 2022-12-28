using Prismarine.NET;
using Prismarine.NET.Networking.Implementations;
using Prismarine.NET.Networking.Interfaces;
using System.Net;
using System.Text.RegularExpressions;

namespace Sandbox
{
    public class SandBoxClass
    {
        static async Task Main(string[] args)
        {
            var client = new PrismarineClient();
            await client.LoginAsync("test-acc");

            var servers = await client.ServerPark.GetAllServers();
            foreach (var item in servers)
            {

                Console.WriteLine(item.Id);
            }
        }
    }
}

