
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.SignalR.Client;
using SharedPublic.DTOs;
using System.Reflection;
using System.Security.Cryptography;

namespace Sandbox
{
    
    public class SandBoxClass
    {
        
        // salt: /VAuVwKNBLbB3XKjMA8J1g==
        // hash: J2bgKuoPeRGc8uhuAsV9gi4W2q454UzPuLufBkTtevM=
        static void Main(string[] args)
        {
            Console.Write("Enter a password: ");
            string? password = Console.ReadLine();

            // Generate a 128-bit salt using a sequence of
            // cryptographically strong random bytes.
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            Console.WriteLine($"Hashed: {hashed}");

        }

        static async Task Main2(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7229/testroute/serverpark")
                .WithAutomaticReconnect()
                .Build();

            connection.Closed += async (e) => Console.WriteLine("Connection closed " + e?.Message);
            connection.On<string>("Receive", (message) => Console.WriteLine(message));
            await connection.StartAsync();
            Console.WriteLine("Connection started");
            
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

