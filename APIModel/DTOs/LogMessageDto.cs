namespace APIModel.DTOs
{
    public class LogMessageDto
    {
        /// <summary>
        /// The message itself.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Type of the log message.
        /// </summary>
        public int MessageType { get; set; }
    }
}