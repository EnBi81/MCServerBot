using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebServer.Config
{
    internal class Config
    {
        private static string ConfigFile => "config.json";
        public static Config Instance { get; } 

        static Config()
        {
            var conf = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFile));
            if (conf == null)
                throw new Exception("Config File Error");

            conf.Check();
            Instance = conf;
        }


#pragma warning disable
        public string HamachiLocation { get; set; } 
        public string DiscordBotToken { get; set; } 
        public string MinecraftServerName { get; set; }
        public string MinecraftServerFile { get; set; }
        public string MinecraftServerProperties { get; set; }
#pragma warning restore


        private void Check()
        {
            if(string.IsNullOrWhiteSpace(HamachiLocation))
                throw new Exception("Please enter a valid value for " + nameof(HamachiLocation));
            if(!Directory.Exists(HamachiLocation))
                throw new Exception(nameof(HamachiLocation) + $" directory does not exist! ({HamachiLocation})");


            if (string.IsNullOrWhiteSpace(DiscordBotToken))
                throw new Exception("Please enter a valid value for " + nameof(DiscordBotToken));


            if (string.IsNullOrWhiteSpace(MinecraftServerName))
                MinecraftServerName = "Unnamed Server";


            if(string.IsNullOrWhiteSpace(MinecraftServerFile))
                throw new Exception("Please enter a valid value for " + nameof(MinecraftServerFile));
            if (!File.Exists(MinecraftServerFile))
                throw new Exception(nameof(MinecraftServerFile) + $" file does not exist! ({MinecraftServerFile})");

            
            if(string.IsNullOrWhiteSpace(MinecraftServerProperties))
                throw new Exception("Please enter a valid value for " + nameof(MinecraftServerProperties));
            if (!File.Exists(MinecraftServerProperties))
                throw new Exception(nameof(MinecraftServerProperties) + $" file does not exist! ({MinecraftServerProperties})");
        }
    }
}
