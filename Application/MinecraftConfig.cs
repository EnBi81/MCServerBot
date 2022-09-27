using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class MinecraftConfig
    {
        private static MinecraftConfig? _instance;
        public static MinecraftConfig Instance { get => _instance ?? throw new Exception("Minecraft Config is not set!"); }

        public static void SetupConfig(string minecraftServersBaseFolder, string javaLocation, string minecraftProcessHandlerPath, int maxRamMB, int initRamMB)
        {
            if (_instance != null)
                return;

            _instance = new MinecraftConfig()
            {
                MinecraftServersBaseFolder = minecraftServersBaseFolder,
                JavaLocation = javaLocation,
                MinecraftServerHandlerPath = minecraftProcessHandlerPath,
                MinecraftServerMaxRamMB = maxRamMB,
                MinecraftServerInitRamMB = initRamMB
            };
        }


        private MinecraftConfig() { }

        public string MinecraftServersBaseFolder { get; private set; } = "";
        public string JavaLocation { get; private set; } = "";
        public string MinecraftServerHandlerPath { get; private set; } = "";
        public int MinecraftServerMaxRamMB { get; private set; }
        public int MinecraftServerInitRamMB { get; private set; }
    }
}
