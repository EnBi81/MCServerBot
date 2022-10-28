using System;
using System.Diagnostics;
using System.IO;

namespace MCServerHandler
{
    internal class Program
    {
        static StreamWriter InputStream;

        //MCServerHandler.exe server.jar "C:\Program Files\Java\jdk-17.0.2\bin\java.exe" "D:\Games\Minecraft Things\McServer" 1024 1024
        static void Main(string[] args)
        {
            // check if we have all the five arguments
            if (args.Length != 5)
            {
                WriteToConsoleAndExit("There must be 5 arguments!");
            }
            
            string simpleFileName = args[0];
            string _javaLocation = args[1];
            string workingDir = args[2];
            int maxRam = int.Parse(args[3]);
            int initRam = int.Parse(args[4]);


            var processStartInfo = new ProcessStartInfo
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                Arguments = $"-Xmx{maxRam}M -Xms{initRam}M -jar \"{simpleFileName}\" nogui",
                FileName = _javaLocation,
                WorkingDirectory = workingDir
            };

            var process = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,
            };


            FileChecker checker = new(workingDir);
            checker.Start();
            checker.FileDeleted += (s, e) => TryStopServer();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => { TryStopServer(); checker.Stop(); };

            
            process.Start();
            Console.WriteLine(process.Id);

            InputStream = process.StandardInput;
            process.BeginOutputReadLine();
            process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
            process.Exited += (sender, args) => Environment.Exit(0);

            while (true)
            {
                string command = Console.ReadLine();
                process.StandardInput.WriteLine(command);
            }
        }

        public static void WriteToConsoleAndExit(string text, int exitCode = 1)
        {
            Console.WriteLine(text);
            Environment.Exit(exitCode);
        }

        public static void TryStopServer()
        {
            try
            {
                InputStream.WriteLine("stop");
            }
            catch { }
        }
    }
}
