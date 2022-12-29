using Microsoft.Extensions.DependencyInjection;
using Prismarine.NET.Model;
using Prismarine.NET.Model.Implementations;
using Prismarine.NET.Networking.Implementations;
using Prismarine.NET.Networking.Interfaces;
using Prismarine.NET.Networking.Utils;

namespace Prismarine.NET
{
    public class PrismarineClient
    {
        private readonly IServiceProvider _serviceProvider;

        public PrismarineClient()
        {
            _serviceProvider = GetServices();
        }
        
        public async Task LoginAsync(string token)
        {
            var authService = _serviceProvider.GetRequiredService<IAuthService>();
            var loginResponse = await authService.Login(token);
            HttpClientProvider provider = _serviceProvider.GetRequiredService<HttpClientProvider>();
            provider.JwtToken = loginResponse.Jwt;
        }



        public IServerPark ServerPark 
        {
            get
            { 
                return _serviceProvider.GetRequiredService<IServerPark>();
            }
        }

        private static IServiceProvider GetServices() => new ServiceCollection()
            // utils
            .AddSingleton<JsonSerializer>()
            .AddSingleton(new HttpClientProvider("https://localhost:7229"))
            // networking
            .AddSingleton<IAuthService, AuthHttpClient>()
            .AddSingleton<IServerParkService, ServerParkHttpClient>()
            .AddSingleton<IMinecraftServerService, MinecraftHttpClient>()
            // models
            .AddSingleton<IServerPark, ServerPark>()
            // build
            .BuildServiceProvider();
    }
}
