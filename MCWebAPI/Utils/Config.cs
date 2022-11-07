using Loggers;
using Loggers.Loggers;
using MCWebAPI.APIExceptions;
using Newtonsoft.Json;
using Shared.Exceptions;

namespace MCWebAPI.Utils
{
    /// <summary>
    /// Handling config file for the whole program.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Json file which holds the config information
        /// </summary>
        private static string ConfigFile => "Resources/config.json";

        /// <summary>
        /// Single config instance
        /// </summary>
        public static Config Instance { get; }

        private static readonly ConfigLogger _logger = LogService.GetService<ConfigLogger>();

        static Config()
        {
            try
            {
                Instance = Setup();
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                throw;
            }
        }

        private static Config Setup()
        {
            _logger.Log("Setting up Config from file " + ConfigFile);

            //check if file exists
            if (!File.Exists(ConfigFile))
            {
                //if file doesnt exist, create a new one and exit the app
                CreateConfigFile();
                throw new ConfigException("Config file not found! A new one is created.");
            }

            //deserialize the json file
            var conf = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFile));
            if (conf == null)
            {
                throw new ConfigException("Could not parse config file. " +
                    "To create a new config file, delete the old one, and restart this program.");
            }

            _logger.Log("Config File Parsed");

            try
            {
                // check if the values are valid
                conf.Check();
            }
            catch (Exception ex)
            {
                throw new ConfigException(ex.Message);
            }

            _logger.Log("Config File Initialized");

            return conf;
        }

        /// <summary>
        /// Creates an empty config file.
        /// </summary>
        private static void CreateConfigFile()
        {
            var config = new Config();
            string json = JsonConvert.SerializeObject(config);

            File.WriteAllText(ConfigFile, json);
        }


#pragma warning disable
        public string HamachiLocation { get; set; }  // location of hamachi exe
        public string MinecraftServerHandlerPath { get; set; }  // server handler exe
        public string MinecraftServersBaseFolder { get; set; }      //folder of the minecraft servers
        public int MinecraftServerMaxRamMB { get; set; }        // max ram for the minecraft server
        public int MinecraftServerInitRamMB { get; set; }       // ram to start the minecraft server with
        public int MinecraftMaxDiskSpaceGB { get; set; } // max disk space for the minecraft servers folder (including the deleted folder as well)
        public string JavaLocation { get; set; }    // java location
        public int WebApiPortHttps { get; set; } // https port for web
        public int WebApiPortHttp { get; set; }  // http port for web

#pragma warning restore


        /// <summary>
        /// Check config fields for invalid values
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Check()
        {
            if (string.IsNullOrWhiteSpace(HamachiLocation))
                throw new ConfigException("Please enter a valid value for " + nameof(HamachiLocation));
            if (!Directory.Exists(HamachiLocation))
                throw new ConfigException(nameof(HamachiLocation) + $" directory does not exist! ({HamachiLocation})");


            if (string.IsNullOrWhiteSpace(MinecraftServersBaseFolder))
                throw new ConfigException("Invalid Minecraft Server Folder Name!");
        }
    }
}
