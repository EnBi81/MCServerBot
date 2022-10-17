namespace Application.Minecraft.Enums
{
    /// <summary>
    /// Represents a single log message, its type and the message itself.
    /// </summary>
    public class LogMessage
    {
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

        /// <summary>
        /// The message itself.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Type of the log message.
        /// </summary>
        public LogMessageType MessageType { get; set; }


        /// <summary>
        /// Initializes both the <see cref="Message"/> and the <see cref="MessageType"/> properties.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public LogMessage(string message, LogMessageType type)
        {
            MessageType = type;
            Message = message;
        }
    }
}
