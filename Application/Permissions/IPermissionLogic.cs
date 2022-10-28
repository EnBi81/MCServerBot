using Shared.DTOs;

namespace Application.Permissions
{
    public interface IPermissionLogic
    {
        public Task RegisterUser(ulong discordId, string? discordUsername, string? profPic);
        public Task<DataUser> GetUser(string token);
        public Task<DataUser> GetUser(ulong id);
        public Task<bool> HasAccess(string token);
        public Task<string> GetToken(ulong id);
    }
}
