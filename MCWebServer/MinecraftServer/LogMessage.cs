namespace MCWebServer.MinecraftServer
{
    public class LogMessage
    {
        public enum LogMessageType
        {
            System_Message, Error_Message, User_Message
        }

        public string Message { get; set; }
        public LogMessageType MessageType { get; set; }


        public LogMessage(string message, LogMessageType type)
        {
            MessageType = type;
            Message = message;
        }
    }
}
