using System.Drawing;
using System.Runtime.CompilerServices;

namespace Loggers
{ 
    /// <summary>
    /// Abstract base logger class.
    /// </summary>
    public abstract class Logger
    {
        /// <summary>
        /// Returns the current time in HH:mm:ss format.
        /// </summary>
        public static string CurrentTime => DateTime.Now.ToString("HH:mm:ss");

        /// <summary>
        /// Writes a log message.
        /// </summary>
        /// <param name="source">source of the message</param>
        /// <param name="log">the actual message</param>
        /// <param name="color">color of the message</param>
        protected static void AddLog(string source, string log, ConsoleColor color = ConsoleColor.Gray)
        {
            string logMessage = $"[{CurrentTime}] {source}: {log}";
            WriteLog(logMessage, color);
        }

        /// <summary>
        /// Writes an exception as a log message.
        /// </summary>
        /// <param name="source">source of the exception</param>
        /// <param name="e">exception</param>
        protected static void AddLogException(string source, Exception e) =>
            AddLog(source, e.ToString(), ConsoleColor.Red);



        private static string? Filename = null;

        /// <summary>
        /// Writes a log message to the console and to the file as well.
        /// </summary>
        /// <param name="logMessage"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void WriteLog(string logMessage, ConsoleColor color)
        {
            if(Filename is null)
            {
                Directory.CreateDirectory("logs");
                Filename = $"logs/{CurrentTime}.txt";
            }

            Console.ForegroundColor = color;
            Console.WriteLine(logMessage, color);
            File.AppendAllText(Filename, logMessage);
        }
    }
}
