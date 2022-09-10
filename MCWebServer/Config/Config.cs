﻿using MCWebServer.Log;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MCWebServer.Config
{
    /// <summary>
    /// Handling config file for the whole program.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Json file which holds the config information
        /// </summary>
        private static string ConfigFile => "config.json";

        /// <summary>
        /// Single config instance
        /// </summary>
        public static Config Instance { get; } 

        static Config()
        {
            LogService.GetService<ConfigLogger>().Log("Setting up Config from file " + ConfigFile);

            //check if file exists
            if (!File.Exists(ConfigFile))
            {
                //if file doesnt exist, create a new one and exit the app
                CreateConfigFile();
                LogService.GetService<ConfigLogger>().LogFatal("Config file not found! A new one is created.");
            }

            //deserialize the json file
            var conf = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFile));
            if (conf == null)
            {
                LogService.GetService<ConfigLogger>().LogFatal("Could not parse config file. " +
                    "To create a new config file, delete the old one, and restart this program.");
            }

            LogService.GetService<ConfigLogger>().Log("Config File Parsed");

            try
            {
                // check if the values are valid
                conf.Check();
            }
            catch (Exception ex)
            {
                LogService.GetService<ConfigLogger>().LogFatal(ex.Message);
            }
            
            Instance = conf;
            LogService.GetService<ConfigLogger>().Log("Config File Initialized");
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
        public string DiscordBotToken { get; set; }  // discord bot token 
        public ulong DiscordBotOwner { get; set; }   // discord bot owner (set when discord starts up)
        public string MinecraftServerHandlerPath { get; set; }  // server handler exe
        public string MinecraftServersFolder { get; set; }      //folder of the minecraft servers
        public int MinecraftServerMaxRamMB { get; set; }        // max ram for the minecraft server
        public int MinecraftServerInitRamMB { get; set; }       // ram to start the minecraft server with
        public int MinecraftServerPerformaceReportDelayInSeconds { get; set; }   // time period to check the minecraft server resource consumption
        public string JavaLocation { get; set; }    // java location
        public int WebServerPortHttps { get; set; } // https port for web
        public int WebServerPortHttp { get; set; }  // http port for web

#pragma warning restore


        /// <summary>
        /// Check config fields for invalid values
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Check()
        {
            if(string.IsNullOrWhiteSpace(HamachiLocation))
                throw new Exception("Please enter a valid value for " + nameof(HamachiLocation));
            if(!Directory.Exists(HamachiLocation))
                throw new Exception(nameof(HamachiLocation) + $" directory does not exist! ({HamachiLocation})");


            if (string.IsNullOrWhiteSpace(DiscordBotToken))
                throw new Exception("Please enter a valid value for " + nameof(DiscordBotToken));


            if (string.IsNullOrWhiteSpace(MinecraftServersFolder))
                throw new Exception("Invalid Minecraft Server Folder Name!");
        }
    }
}
