using System;

namespace Loggers
{
    public class NetworkLogger : Logger
    {
        public void Log(string text)
        {
            var log = $"[{CurrentTime}] Networking: {text}";
            WriteLog(log, ConsoleColor.White);
        }

        public void LogFatal(string text)
        {
            var log = $"[{CurrentTime}] Networking-fatal: {text}";
            WriteLog(log, ConsoleColor.Red);
            WriteLog("Press any button to exit...");

            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
