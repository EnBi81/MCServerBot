using System.IO;
using System.Runtime.CompilerServices;

namespace MCWebServer
{
    internal class Log
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LogMessage(string mess)
        {
            using var writer = new StreamWriter("log-text.txt", true);
            writer.WriteLine(mess);
        }
    }
}
