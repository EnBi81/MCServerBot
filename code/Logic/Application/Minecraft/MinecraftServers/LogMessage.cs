using Shared.Model;
using static Shared.Model.ILogMessage;

namespace Application.Minecraft.MinecraftServers
{
    /// <summary>
    /// Represents a single log message, its type and the message itself.
    /// </summary>
    public class LogMessage : ILogMessage
    {

        /// <inheritdoc/>
        public string Message { get; set; }
        /// <inheritdoc/>
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
