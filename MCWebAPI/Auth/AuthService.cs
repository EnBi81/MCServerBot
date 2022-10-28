using Application.DAOs;
using Application.DAOs.Database;
using Application.Permissions;
using Shared.DTOs;
using SharedAuth.DTOs;

namespace MCWebAPI.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IPermissionLogic _permissionLogic;

        public AuthService(IPermissionLogic permissionLogic)
        {
            _permissionLogic = permissionLogic;
        }


        public async Task<DataUser> GetUser(string? token)
        {
            DataUser? user = await _permissionLogic.GetUser(token);

            if (user == null)
                throw new Exception("User with token {} is not registered.");

            return user;
        }

    }
}
