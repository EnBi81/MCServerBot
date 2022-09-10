namespace MCWebServer.Log
{
    public class MinecraftLogger : Logger
    {
        public void Log(string source, string message, System.ConsoleColor color = System.ConsoleColor.Green)
        {
            var log = $"[{CurrentTime}] Minecraft-{source}: {message}";
            WriteLog(log, color);
        }
    }
}
