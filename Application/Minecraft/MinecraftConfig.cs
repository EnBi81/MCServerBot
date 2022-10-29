namespace Application.Minecraft
{
    /// <summary>
    /// Config data for the minecraft servers
    /// </summary>
    public class MinecraftConfig
    {
        /// <summary>
        /// Folder where the base minecraft folders are located.
        /// </summary>
        public string MinecraftServersBaseFolder { get; init; } = "";
        /// <summary>
        /// Path to java.exe
        /// </summary>
        public string JavaLocation { get; init; } = "";
        /// <summary>
        /// Path to the serverhandler
        /// </summary>
        public string MinecraftServerHandlerPath { get; init; } = "";
        /// <summary>
        /// Max ram a minecraft server can take
        /// </summary>
        public int MinecraftServerMaxRamMB { get; init; }
        /// <summary>
        /// Ram the server has when starts
        /// </summary>
        public int MinecraftServerInitRamMB { get; init; }
        /// <summary>
        /// Max 
        /// </summary>
        public int MaxSumOfDiskSpaceGB { get; init; }
    }
}
