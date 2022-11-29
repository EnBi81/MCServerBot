using SharedPublic.DTOs;

namespace Application.Permissions
{
    public interface IPermissionLogic
    {
        public Task RegisterUser(ulong discordId, string? discordUsername, string? profPic);
        public Task<DataUser> GetUser(string? token);
        public Task<DataUser> GetUser(ulong id);
        public Task<bool> HasAccess(string? token);
        public Task<string> GetToken(ulong id);
        public Task RefreshUser(ulong discordId, string? discordUsername, string? profPic);
        public Task GrantPermission(ulong discordId, UserEventData userEventData);
        public Task RevokePermission(ulong discordId, UserEventData userEventData);



        public event EventHandler<DataUser> PermissionRevoked;
    }
}
