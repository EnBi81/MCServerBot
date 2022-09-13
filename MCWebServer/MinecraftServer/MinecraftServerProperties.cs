using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MCWebServer.MinecraftServer
{
    /// <summary>
    /// Class managing the properties file for a minecraft server.
    /// </summary>
    public class MinecraftServerProperties
    {
        /// <summary>
        /// Reads the file specified in the argument and creates a new instance of the MinecraftServerProperties.
        /// </summary>
        /// <param name="file">Path to the properties file.</param>
        /// <returns>Information of the properties loaded into a MinecraftServerProperties instance.</returns>
        public static MinecraftServerProperties GetProperties(string file)
        {
            string[] lines = File.ReadAllLines(file);
            return new MinecraftServerProperties(lines);
        }

        /// <summary>
        /// Save a MinecraftServerProperties instance to a file.
        /// </summary>
        /// <param name="file">path to save the file</param>
        /// <param name="props">instance to save</param>
        public static void SaveProperties(string file, MinecraftServerProperties props)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var (key, value) in props)
                sb.AppendLine(key + "=" + value.ToString());

            File.WriteAllText(file, sb.ToString());
        }



        
        /// <summary>
        /// Dictionary of each of the property key and value.
        /// </summary>
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Initializes the instance by splitting the lines to key value pairs and puts them into the Properties
        /// </summary>
        /// <param name="lines"></param>
        public MinecraftServerProperties(IEnumerable<string> lines)
        {
            Regex regex = new Regex("[^=]=[^=]");
            foreach(var line in lines)
            {
                if(!regex.IsMatch(line))
                    continue;

                string[] parts = line.Split('=');
                string key = parts[0];
                string value = parts[1];

                Properties.Add(key, value);
            }
        }

        public string this[string key]
        {
            get => Properties[key];
            set => Properties[key] = value;
        }

        public Dictionary<string, string>.Enumerator GetEnumerator()
        {
            return Properties.GetEnumerator();
        }
    }
}
