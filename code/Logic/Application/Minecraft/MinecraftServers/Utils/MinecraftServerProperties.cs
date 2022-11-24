using Shared.DTOs;
using Shared.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Minecraft.MinecraftServers.Utils
{
    /// <summary>
    /// Class managing the properties file for a minecraft server.
    /// </summary>
    public class MinecraftServerProperties : IMinecraftServerProperties
    {
        /// <summary>
        /// Reads the file specified in the argument and creates a new instance of the MinecraftServerProperties.
        /// </summary>
        /// <param name="file">Path to the properties file.</param>
        /// <returns>Information of the properties loaded into a MinecraftServerProperties instance.</returns>
        public static MinecraftServerProperties GetProperties(string file)
        {
            return new MinecraftServerProperties(file);
        }

        /// <summary>
        /// Save a MinecraftServerProperties instance to a file.
        /// </summary>
        /// <param name="file">path to save the file</param>
        /// <param name="props">instance to save</param>
        public static async Task SaveProperties(string file, MinecraftServerProperties props)
        {
            StringBuilder sb = new();
            foreach (var (key, value) in props.Properties)
                sb.AppendLine(key + "=" + value.ToString());

            await File.WriteAllTextAsync(file, sb.ToString());
        }




        /// <inheritdoc/>
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        private readonly string _file;

        /// <summary>
        /// Initializes the instance by splitting the lines to key value pairs and puts them into the Properties
        /// </summary>
        /// <param name="lines"></param>
        public MinecraftServerProperties(string file)
        {
            _file = file;
            
            
        }


        private void LoadData()
        {
            if (Properties.Any())
                return;
            
            string[] lines;
            try
            {
                lines = File.ReadAllLines(_file);
            }
            catch (FileNotFoundException)
            {
                lines = Array.Empty<string>();
            }
            
            Regex regex = new("[^=]+=[^=]*");
            foreach (var line in lines)
            {
                if (!regex.IsMatch(line))
                    continue;

                string[] parts = line.Split('=');
                string key = parts[0];
                string value = parts[1];

                Properties.Add(key, value);
            }
        }
        

        /// <inheritdoc/>
        public string? this[string key]
        {
            get 
            {
                LoadData();
                Properties.TryGetValue(key, out string? value);
                return value;
            }
        }

        /// <inheritdoc/>
        public Dictionary<string, string>.Enumerator GetEnumerator()
            => Properties.GetEnumerator();

        public async Task UpdateProperties(MinecraftServerPropertiesDto dto)
        {
            LoadData();

            foreach (var (key, value) in dto.ValidateAndRetrieveData())
            {
                if (Properties.ContainsKey(key))
                    Properties[key] = value;
            }

            await SaveProperties(_file, this);
        }
    }
}
