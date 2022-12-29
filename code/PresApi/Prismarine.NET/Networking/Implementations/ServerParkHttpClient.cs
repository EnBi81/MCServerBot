using Prismarine.NET.DTOs;
using Prismarine.NET.Model;
using Prismarine.NET.Model.Implementations;
using Prismarine.NET.Networking.Abstract;
using Prismarine.NET.Networking.Interfaces;
using Prismarine.NET.Networking.Utils;

namespace Prismarine.NET.Networking.Implementations
{
    internal class ServerParkHttpClient : BaseController, IServerParkService
    {
        private const string BaseUri = "/api/v1/serverpark";

        public ServerParkHttpClient(HttpClientProvider httpClientProvider, JsonSerializer jsonSerializer) : base(httpClientProvider, jsonSerializer)
        {
        }

        public async Task<IMinecraftServer> CreateServer(ServerCreationDto dto) 
        {
            var server = await PostAsync<MinecraftServer>(BaseUri, dto);
            return server;
        }

        public async Task<IEnumerable<IMinecraftServer>> GetAllServers()
        {
            ICollection<MinecraftServer> servers = await GetAsync<ICollection<MinecraftServer>>(BaseUri);
            return servers.Cast<IMinecraftServer>();
        }

        public async Task<ICollection<MinecraftVersion>> GetMinecraftVersions()
        {
            var versions = await GetAsync<ICollection<MinecraftVersion>>(BaseUri + "/versions");
            return versions;
        }
    }
}
