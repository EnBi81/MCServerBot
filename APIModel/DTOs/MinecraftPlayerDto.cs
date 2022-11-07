namespace APIModel.DTOs
{
    public class MinecraftPlayerDTO
    {
        /// <summary>
        /// Username of the player
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// If the player is online, this holds the time joined to the server, else null.
        /// </summary>
        public DateTime? OnlineFrom { get; set; }

        /// <summary>
        /// Sum of the time spent online from the previous sessions.
        /// </summary>
        public TimeSpan PastOnline { get; set; }
    }
}