using System;
using System.Runtime.CompilerServices;

namespace MCWebServer.Log
{
    public abstract class Logger
    {
        public static string CurrentTime => DateTime.Now.ToString("HH:mm:ss");

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected static void WriteLog(string log, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(log);
        }
    }
}
