namespace Application.Minecraft.Enums
{
    /// <summary>
    /// Represents the status of a server
    /// </summary>
    public enum ServerStatus
    {
        /// <summary>
        /// Server process is not running, the server is offline.
        /// </summary>
        Offline,

        /// <summary>
        /// Server process has been started, it is not ready though.
        /// </summary>
        Starting,

        /// <summary>
        /// Server fully functions, it's online
        /// </summary>
        Online,
        
        /// <summary>
        /// Server process is shutting down, expect to go offline in some seconds.
        /// </summary>
        ShuttingDown
    }

    /// <summary>
    /// Extension class for ServerStatus enum
    /// </summary>
    internal static class ServerStatusExtensions
    {
        /// <summary>
        /// Converts the enum variable to string
        /// </summary>
        /// <param name="status"></param>
        /// <returns>the string representative of the enum value</returns>
        public static string DisplayString(this ServerStatus status)
        {
            if (status == ServerStatus.Online)
                return $"Server Online on {HamachiHelper.HamachiClient.Address}:{ServerPark.ActiveServer?.Port}";
            if (status == ServerStatus.Offline)
                return "Server Offline";
            if (status == ServerStatus.Starting)
                return "Server Starting";
            if (status == ServerStatus.ShuttingDown)
                return "Server Shutting Down";

            return "";
        }
    }
}
