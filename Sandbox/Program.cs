using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace Sandbox
{
    public class SandBoxClass
    {
        static void Main(string[] args)
        {
            StartServer();
        }

        static void StartServer()
        {
            var _serverHandlerPath = "C:\\Users\\enbi8\\source\\repos\\MCServerBot\\MCServerHandler\\bin\\Debug\\net5.0\\MCServerHandler.exe";
            var _javaLocation = "C:\\Program Files\\Java\\jdk-17.0.2\\bin\\java.exe";

            FileInfo info = new FileInfo("A:\\MinecraftServerTest\\jar files\\server.jar");
            var workingDir = "A:\\MinecraftServerTest\\Empty Server";
            var simpleFileName = "A:\\MinecraftServerTest\\jar files\\server.jar";

            var processStartInfo = new ProcessStartInfo
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = false,
                //Arguments = $"\"{serverHandlerPath}\" {simpleFileName} \"{_javaLocation}\" \"{workingDir}\" {maxRam} {initRam}",
                FileName = "cmd.exe",
                WorkingDirectory = workingDir
            };

            var _serverHandlerProcess = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,
            };

           
            _serverHandlerProcess.Start();
            _serverHandlerProcess.StandardInput.WriteLine($"\"{_serverHandlerPath}\" \"{simpleFileName}\" \"{_javaLocation}\" \"{workingDir}\" {8000} {8000} & exit");
            _serverHandlerProcess.BeginErrorReadLine();
            _serverHandlerProcess.BeginOutputReadLine();



            int messageCount = 0;
            _serverHandlerProcess.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null)
                    return;

                Console.WriteLine(e.Data);
            };

            _serverHandlerProcess.WaitForExit();
        }
    }
}

