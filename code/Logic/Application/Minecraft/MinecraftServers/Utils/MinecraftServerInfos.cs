using Newtonsoft.Json;
using Shared.DTOs;
using Shared.Exceptions;
using Shared.Model;

namespace Application.Minecraft.MinecraftServers.Utils
{
    /// <summary>
    /// Minecraft server info collection. This class is responsible for saving and retrieving data of a minecraft server from a file.
    /// </summary>
    internal class MinecraftServerInfos
    {
        [JsonIgnore]
        private readonly string _filename; // absolute path and file name of the info file.

        /// <summary>
        /// Id of the server.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name of the server.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Minecraft version of the server.
        /// </summary>
        public string Version { get; set; } = null!;

        /// <summary>
        /// If the minecraft server is in maintenance mode
        /// </summary>
        public bool IsMaintenance { get; set; }

        /// <summary>
        /// Properties to apply when the server is created
        /// </summary>
        public MinecraftServerCreationPropertiesDto CreationProperties { get; set; }


        /// <summary>
        /// This constructor is for json deserialization. pls dont delete it, ty.
        /// </summary>
        private MinecraftServerInfos()
        {
            // for json
            _filename = null!;
        }

        /// <summary>
        /// Initializes the server info file handler object.
        /// </summary>
        /// <param name="filename"></param>
        public MinecraftServerInfos(string filename)
        {
            _filename = filename;
        }


        /// <summary>
        /// Saves information of a server into the server info file.
        /// </summary>
        /// <param name="server">server to save.</param>
        public void Save(MinecraftServerLogic server)
        {
            Id = server.Id;
            Name = server.ServerName;
            Version = server.MCVersion.Version;
            IsMaintenance = server.StatusCode == ServerStatus.Maintenance;
            CreationProperties = server.CreationProperties;

            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(_filename, json);
        }

        /// <summary>
        /// Loads server infomation from a file.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Load()
        {
            string json = File.ReadAllText(_filename);
            JsonSerializerSettings settings = new()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            var obj = JsonConvert.DeserializeObject<MinecraftServerInfos>(json, settings);
            if (obj == null)
                throw new Exception("Minecraft server info file is invalid");
            
            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.GetValue(obj) is null)
                    throw new MCInternalException($"Minecraft server {_filename} info file has invalid porperty: {property.Name}");
            }
            

            Id = obj.Id;
            Name = obj.Name;
            Version = obj.Version;
            IsMaintenance = obj.IsMaintenance;
            CreationProperties = obj.CreationProperties;
        }
    }
}
