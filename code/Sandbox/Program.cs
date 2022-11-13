
using Microsoft.AspNetCore.SignalR.Client;
using System.Reflection;

namespace Sandbox
{
    
    public class SandBoxClass
    {

        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7229/testroute/serverpark")
                .WithAutomaticReconnect()
                .Build();

            connection.Closed += async (e) => Console.WriteLine("Connection closed " + e?.Message);
            connection.On<string>("Receive", (message) => Console.WriteLine(message));
            await connection.StartAsync();
            Console.WriteLine("Connection started");

            await connection.SendAsync("Receive", "CustomClient");
            await Task.Delay(-1);
        }


        static void Main1(string[] args) 
        {
            var types = typeof(SandBoxClass).Assembly.DefinedTypes
                .Where(t => t.GetCustomAttribute<CommandAttribute>() is not null);

            List<string> commands = new();

            foreach (var type in types)
            {
                commands.Add(type.GetCustomAttribute<CommandAttribute>().Name);
                Console.WriteLine(type.FullName);
            }

            Console.WriteLine("Enter a command");
            string input = "";

            int longestWritten = 0;
            int consolePos = Console.CursorTop;
            
            while (true)
            {
                Console.CursorTop = consolePos;
                Console.CursorLeft = 0;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(input);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                int posX = Console.CursorLeft;
                var commandTips = commands.Where(c => c.StartsWith(input, StringComparison.OrdinalIgnoreCase));
                Console.Write(commandTips.FirstOrDefault()?.Substring(Console.CursorLeft));

                if (longestWritten > Console.CursorLeft)
                {
                    var longestWrittenTemp = Console.CursorLeft;
                    while (longestWritten > Console.CursorLeft)
                        Console.Write(" ");

                    longestWritten = longestWrittenTemp;
                }
                else longestWritten = Console.CursorLeft;
                

                Console.CursorLeft = posX;

                var key = Console.ReadKey();

                if (key.Key != ConsoleKey.Backspace)
                    input += key.KeyChar;
                else if (input.Length > 0)
                    input = input[..^1];
            }

            
        }
    }
    



    [Command("scoreboard")]
    [ArgumentList("<message:string>")]
    public class MeCommand
    {
        
    }

    [Command("seed")]
    public class SeedCommand
    {
        
    }

    [Command("meandalonso")]
    public class SeedaCommand
    {

    }
    

    public class CommandAttribute : Attribute
    {
        public string Name { get; }

        public CommandAttribute(string command) => Name = command;        
    }

    public class ArgumentListAttribute : Attribute
    {
        public ArgumentListAttribute(string argumentList) { }
    }
}

