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

            var versions = await client.ServerPark.GetMinecraftVersionsAsync();
            foreach (var version in versions)
            {
                Console.WriteLine(version.Name);
                Console.WriteLine(version.Version);
                Console.WriteLine();
            }

            //var servers = await client.ServerPark.GetAllServersAsync();
            //foreach (var item in servers)
            //{
            //    // write out all the properties
            //    Console.WriteLine("id: " + item.Id);
            //    Console.WriteLine("name: " + item.ServerName);
            //    Console.WriteLine("is running: " + item.IsRunning);
            //    Console.WriteLine("version: " + item.MCVersion.Version);
            //    Console.WriteLine("logs: ");
            //    foreach (var log in item.Logs)
            //    {
            //        Console.WriteLine(" - " + log.Message);
            //    }

            //    Console.WriteLine("status: " + item.Status);

            //    Console.WriteLine();
            //}
        }
    }
}

