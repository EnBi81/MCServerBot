using APIModel.DTOs;
using APIModel.Responses;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;

namespace MCWebAPI.Auth
{
    public interface IAuthService
    {
        Task<AuthenticatedResponse> Login(LoginDto dto);
    }
}
