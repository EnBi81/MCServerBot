namespace Prismarine.NET.Model
{
    public class LogMessage
    {
        public string Message { get; set; }
        public LogMessageType MessageType { get; set; }
    }
    
    /// <summary>
    /// The types of a log message.
    /// </summary>
    public enum LogMessageType
    {
        /// <summary>
        /// System message created by either the minecraft server or this system.
        /// </summary>
        System_Message,
        /// <summary>
        /// Error message created by either the minecraft server or this system.
        /// </summary>
        Error_Message,
        /// <summary>
        /// User message created by the user.
        /// </summary>
        User_Message
    }
}