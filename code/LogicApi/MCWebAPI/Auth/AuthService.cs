using APIModel.DTOs;
using APIModel.Responses;
using Application.DAOs;
using Application.DAOs.Database;
using Application.Permissions;
using MCWebAPI.APIExceptions;
using MCWebAPI.Controllers.Utils;
using SharedPublic.DTOs;
using SharedPublic.DTOs.Enums;

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

            DataUser user = await GetUser(dto.Token);
            string token = AuthUtils.GenerateJwt(user, Platform.Website, _config);

            return new AuthenticatedResponse { JWT = token, Type = "Bearer" };
        }

    }
}
