using Prismarine.NET.Model;
using Prismarine.NET.Model.Implementations;
using Prismarine.NET.Networking.Abstract;
using Prismarine.NET.Networking.Interfaces;
using Prismarine.NET.Networking.Utils;

namespace Prismarine.NET.Networking.Implementations
{
    internal class MinecraftHttpClient : BaseController, IMinecraftServerService
    {
        private const string BasePath = "/api/v1/minecraftserver";
        private string GetPathById(long id, params string[] additionalRoutes) => $"{BasePath}/{id}/{string.Join("/", additionalRoutes)}";


        public MinecraftHttpClient(HttpClientProvider httpClientProvider, JsonSerializer jsonSerializer) : base(httpClientProvider, jsonSerializer)
        {
        }

        public async Task<IMinecraftServer> GetServerById(long id)
        {
            var path = GetPathById(id);
            return await GetAsync<MinecraftServer>(path);
        }
    }
}
