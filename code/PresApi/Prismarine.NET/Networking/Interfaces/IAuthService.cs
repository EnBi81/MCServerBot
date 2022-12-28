using Prismarine.NET.DTOs;

namespace Prismarine.NET.Networking.Interfaces;

internal interface IAuthService
{
    /// <summary>
    /// Gets a jwt token from a login token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<AuthenticatedResponse> Login(string token);
}
