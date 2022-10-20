using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Minecraft.MinecraftServers
{
    internal class MinecraftServerInfos
    {
        [JsonIgnore]
        private readonly string _filename;

        public ulong Id { get; set; }
        public string? Name { get; set; }


        private MinecraftServerInfos()
        {
            // for json
            _filename = null!;
        }

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
