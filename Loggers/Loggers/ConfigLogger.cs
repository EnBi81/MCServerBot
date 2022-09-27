using System;

namespace Loggers
{
    public class ConfigLogger : Logger
    {
        public void Log(string text)
        {
            var log = $"[{CurrentTime}] Config: {text}";
            WriteLog(log, ConsoleColor.White);
        }

        public void LogFatal(string text)
        {
            var log = $"[{CurrentTime}] Config-fatal: {text}";
            WriteLog(log, ConsoleColor.Red);
            WriteLog("Press any button to exit...");

            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
