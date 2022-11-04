namespace Loggers
{
    public class MinecraftLogger : Logger
    {
        public string ServerPark => "serverpark";
        public string MinecraftServer => "mcserver";

        public void Log(string source, string message) =>
            AddLog("Minecraft-" + source, message, ConsoleColor.Green);

        public void Error(string source, Exception e) =>
            AddLogException(source, e);
    }
}
