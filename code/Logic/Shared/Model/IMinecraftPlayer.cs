
namespace SharedPublic.Model
{
    /// <summary>
    /// Holds information of a minecraft player in
    /// </summary>
    public interface IMinecraftPlayer
    {
        /// <summary>
        /// Username of the player
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// If the player is online, this holds the time joined to the server, else null.
        /// </summary>
        public DateTime? OnlineFrom { get; }

        /// <summary>
        /// Sum of the time spent online from the previous sessions.
        /// </summary>
        public TimeSpan PastOnline { get; }
    }
}
