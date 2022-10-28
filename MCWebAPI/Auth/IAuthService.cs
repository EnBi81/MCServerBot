using Discord;
using Shared.DTOs;
using SharedAuth.DTOs;

namespace MCWebAPI.Auth
{
    public interface IAuthService
    {
        Task<DataUser> GetUser(string? token);
    }
}
