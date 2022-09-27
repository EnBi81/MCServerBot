namespace Application
{
    /// <summary>
    /// Config data for the minecraft servers
    /// </summary>
    public class MinecraftConfig
    {
        private static MinecraftConfig? _instance;
        public static MinecraftConfig Instance { get => _instance ?? throw new Exception("Minecraft Config is not set!"); }

        /// <summary>
        /// Sets up the config instance
        /// </summary>
        /// <param name="minecraftServersBaseFolder"><see cref="MinecraftServersBaseFolder"/></param>
        /// <param name="javaLocation"><see cref="JavaLocation"/></param>
        /// <param name="minecraftProcessHandlerPath"><see cref="MinecraftServerHandlerPath"/></param>
        /// <param name="maxRamMB"><see cref="MinecraftServerMaxRamMB"/></param>
        /// <param name="initRamMB"><see cref="MinecraftServerInitRamMB"/></param>
        public static void SetupConfig(string minecraftServersBaseFolder, string javaLocation, string minecraftProcessHandlerPath, int maxRamMB, int initRamMB, int maxStorageGB)
        {
            if (_instance != null)
                return;

            _instance = new MinecraftConfig()
            {
                MinecraftServersBaseFolder = minecraftServersBaseFolder,
                JavaLocation = javaLocation,
                MinecraftServerHandlerPath = minecraftProcessHandlerPath,
                MinecraftServerMaxRamMB = maxRamMB,
                MinecraftServerInitRamMB = initRamMB,
                MaxSumOfDiskSpaceGB = maxStorageGB
            };
        }


        private MinecraftConfig() { }

        /// <summary>
        /// Folder where the base minecraft folders are located.
        /// </summary>
        public string MinecraftServersBaseFolder { get; private set; } = "";
        /// <summary>
        /// Path to java.exe
        /// </summary>
        public string JavaLocation { get; private set; } = "";
        /// <summary>
        /// Path to the serverhandler
        /// </summary>
        public string MinecraftServerHandlerPath { get; private set; } = "";
        /// <summary>
        /// Max ram a minecraft server can take
        /// </summary>
        public int MinecraftServerMaxRamMB { get; private set; }
        /// <summary>
        /// Ram the server has when starts
        /// </summary>
        public int MinecraftServerInitRamMB { get; private set; }
        /// <summary>
        /// Max 
        /// </summary>
        public int MaxSumOfDiskSpaceGB { get; private set; }
    }
}
