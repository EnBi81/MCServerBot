namespace Application.Minecraft.Configs
{
    /// <summary>
    /// Config data for the minecraft servers
    /// </summary>
    public class MinecraftConfig
    {
        /// <summary>
        /// Folder where the base minecraft folders are located.
        /// </summary>
        public required string MinecraftServersBaseFolder { get; init; }
        /// <summary>
        /// Path to java.exe
        /// </summary>
        public required string JavaLocation { get; init; }
        /// <summary>
        /// Path to the serverhandler
        /// </summary>
        public required string MinecraftServerHandlerPath { get; init; }
        /// <summary>
        /// Max disk space the servers can take
        /// </summary>
        public required int MaxSumOfDiskSpaceGB { get; init; }

        /// <summary>
        /// Folder to store the backups
        /// </summary>
        public required string BackupFolder { get; init; }

        /// <summary>
        /// Minecraft server config
        /// </summary>
        public required MinecraftServerConfig ServerConfig { get; init; }
    }
}
