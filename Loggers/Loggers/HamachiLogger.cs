namespace Loggers
{
    public class HamachiLogger : Logger
    {
        public void Log(string message)
        {
            var log = $"[{CurrentTime}] Hamachi: {message}";
            WriteLog(log, System.ConsoleColor.Cyan);
        }
    }
}
