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
    }
}
