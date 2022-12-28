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
            Console.WriteLine("logged in");
        }



        public IServerPark ServerPark 
        {
            get
            { 
                return _serviceProvider.GetRequiredService<IServerPark>();
            }
        }

        private static IServiceProvider GetServices() => new ServiceCollection()
            .AddSingleton(JsonSerializer.Instance)
            .AddSingleton(new HttpClientProvider("https://localhost:7229"))
            .AddSingleton<IAuthService, AuthHttpClient>()
            .AddSingleton<IServerParkService, ServerParkHttpClient>()
            .AddSingleton<IServerPark, ServerPark>()
            
            .BuildServiceProvider();
    }
}
