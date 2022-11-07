using APIModel.APIExceptions;
using APIModel.DTOs;
using APIModel.Responses;
using Application.DAOs;
using Application.DAOs.Database;
using Application.Permissions;
using MCWebAPI.Controllers.Utils;
using Shared.DTOs;
using Shared.DTOs.Enums;

namespace MCWebAPI.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IPermissionLogic _permissionLogic;
        private readonly IConfiguration _config;

        public AuthService(IPermissionLogic permissionLogic, IConfiguration config)
        {
            _permissionLogic = permissionLogic;
            _config = config;
        }


        public async Task<DataUser> GetUser(string? token)
        {
            DataUser? user = await _permissionLogic.GetUser(token);

            if (user == null)
                throw new Exception("User with token {} is not registered.");

            return user;
        }

        public async Task<AuthenticatedResponse> Login(LoginDto dto)
        {
            foreach (var prop in dto.GetType().GetProperties())
            {
                if (prop.GetValue(dto) is null)
                    throw new LoginException("You must provide a " + prop.Name);
            }

            if (!Enum.TryParse(dto.Platform, true, out Platform platform))
                throw new LoginException("Unrecognized platform: " + dto.Platform);

            DataUser user = await GetUser(dto.Token);
            string token = AuthUtils.GenerateJwt(user, platform, _config);

            return new AuthenticatedResponse { Token = token, Type = "Bearer" };
        }

    }
}
