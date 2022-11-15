namespace SharedPublic.Model
{
    /// <summary>
    /// Represents a single log message, its type and the message itself.
    /// </summary>
    public interface ILogMessage
    {
        /// <summary>
        /// The message itself.
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// Type of the log message.
        /// </summary>
        public LogMessageType MessageType { get; }
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
