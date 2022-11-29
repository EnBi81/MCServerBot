using APIModel.DTOs;
using APIModel.Responses;
using Microsoft.AspNetCore.Identity;
using SharedPublic.DTOs;

namespace MCWebAPI.Auth
{
    public interface IAuthService
    {
        Task<AuthenticatedResponse> Login(LoginDto dto);
    }
}
