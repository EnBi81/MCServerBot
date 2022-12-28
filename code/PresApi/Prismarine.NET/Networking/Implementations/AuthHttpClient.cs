using Prismarine.NET.DTOs;
using Prismarine.NET.Networking.Abstract;
using Prismarine.NET.Networking.Interfaces;

namespace Prismarine.NET.Networking.Implementations
{
    public class AuthHttpClient : BaseController, IAuthService
    {
        public async Task<AuthenticatedResponse> Login(string token)
        {
            return await PostAsync<AuthenticatedResponse>("/api/v1/auth/login", new { token });
        }
    }
}
