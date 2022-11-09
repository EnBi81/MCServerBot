using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.Util
{
    internal class MinecraftEnvironment
    {
        public string BaseDirectory { get; }
        public string ServersDataDirectory { get; }
        public string ServerJarDirectory { get; }
        public string DeletedServersDirectory { get; }
        

        public MinecraftEnvironment(string mcBaseDir)
        {
            BaseDirectory = mcBaseDir;
            ServersDataDirectory = Path.Combine(BaseDirectory, "Servers");
            ServerJarDirectory = Path.Combine(BaseDirectory, "ServerJars");
            DeletedServersDirectory = Path.Combine(BaseDirectory, "DeletedServers");

            CreateDirectories();
        }

        private void CreateDirectories()
        {
            foreach (var dir in GetType().GetProperties().Where(p => p.Name.EndsWith("Directory")))
            {
                Directory.CreateDirectory((string)dir.GetValue(this)!);
            }
        }
    }
}
