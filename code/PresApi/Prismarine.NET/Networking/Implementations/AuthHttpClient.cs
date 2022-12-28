using Microsoft.Extensions.DependencyInjection;
using Prismarine.NET.DTOs;
using Prismarine.NET.Networking.Abstract;
using Prismarine.NET.Networking.Interfaces;
using Prismarine.NET.Networking.Utils;

namespace Prismarine.NET.Networking.Implementations
{
    internal class AuthHttpClient : BaseController, IAuthService
    {
        public AuthHttpClient(HttpClientProvider httpClientProvider, JsonSerializer jsonSerializer) : base(httpClientProvider, jsonSerializer)
        {
        }

        public async Task<AuthenticatedResponse> Login(string token)
        {
            return await PostAsync<AuthenticatedResponse>("/api/v1/auth/login", new { token });
        }
    }
}
