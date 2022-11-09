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
        protected static void AddLog(string source, string log, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            string logMessage = $"[{CurrentTime}] {source}: {log}";
            WriteLog(logMessage, color, bgColor);
        }

        /// <summary>
        /// Writes an exception as a log message.
        /// </summary>
        /// <param name="source">source of the exception</param>
        /// <param name="e">exception</param>
        protected static void AddLogException(string source, Exception e, bool fatal = false) =>
            AddLog(source, e.ToString(), fatal ? ConsoleColor.White : ConsoleColor.Red, fatal ? ConsoleColor.Red : ConsoleColor.Black);



        private static string? Filename = null;

        /// <summary>
        /// Writes a log message to the console and to the file as well.
        /// </summary>
        /// <param name="logMessage"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void WriteLog(string logMessage, ConsoleColor color, ConsoleColor bgColor)
        {
            if(Filename is null)
            {
                string baseDir = (Environment.GetEnvironmentVariable("RESOURCES_FOLDER") + "/" ?? "") + "Logs";
                Directory.CreateDirectory(baseDir);
                Filename = $"{baseDir}/{DateTime.Now:yyyy-MM-dd-HH-mm-ss-FFFF}.txt";
            }

            Console.ForegroundColor = color;
            Console.WriteLine(logMessage);
            File.AppendAllText(Filename, logMessage + Environment.NewLine);
        }
    }
}
