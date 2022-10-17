using Discord;
using System;

namespace Loggers
{
    public class DiscordLogger : Logger
    {
        public void Log(LogMessage message)
        {
            var log = $"[{CurrentTime}] Discord: {$"({message.Severity})",-10} {message.Source,-10}: {message.Message}";
            WriteLog(log, ConsoleColor.Magenta);
        }

        public void LogError(string text)
        {
            var log = $"[{CurrentTime}] Discord-error: {text}";
            WriteLog(log, ConsoleColor.Red);
        }

        public void LogFatal(string text)
        {
            var log = $"[{CurrentTime}] Discord-fatal: {text}";
            WriteLog(log, ConsoleColor.Red);
            WriteLog("Press any button to exit...");

            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
