using Newtonsoft.Json;

namespace Application.Minecraft.MinecraftServers
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
        public ulong Id { get; set; }

        /// <summary>
        /// Name of the server.
        /// </summary>
        public string Name { get; set; } = null!;


        /// <summary>
        /// This constructor is for json deserialization. pls dont delete it, ty.
        /// </summary>
        private MinecraftServerInfos()
        {
            // for json
            _filename = null!;
        }

        /// <summary>
        /// Creates
        /// </summary>
        /// <param name="filename"></param>
        public MinecraftServerInfos(string filename)
        {
            _filename = filename;
        }

        
        public void Save(IMinecraftServer server)
        {
            Id = server.Id;
            Name = server.ServerName;

            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(_filename, json);
        }

        public void Load()
        {
            string json = File.ReadAllText(_filename);
            JsonSerializerSettings settings = new ()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            var obj = JsonConvert.DeserializeObject<MinecraftServerInfos>(json, settings);
            if (obj == null)
                throw new Exception("Minecraft server info file is invalid");

            Id = obj.Id;
            Name = obj.Name;
        }
    }
}
