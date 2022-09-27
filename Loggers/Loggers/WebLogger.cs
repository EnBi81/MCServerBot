namespace Loggers 
{ 
    public class WebLogger : Logger
    {
        public void Log(string source, string message)
        {
            var log = $"[{CurrentTime}] Web-{source}: {message}";
            WriteLog(log, System.ConsoleColor.Yellow);
        }
    }
}
