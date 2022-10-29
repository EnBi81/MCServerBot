using Discord;
using Shared.DTOs;

namespace MCWebAPI.Auth
{
    public interface IAuthService
    {
        Task<DataUser> GetUser(string? token);
    }
}
