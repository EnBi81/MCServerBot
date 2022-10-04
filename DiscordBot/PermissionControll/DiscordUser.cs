using Discord;
using Newtonsoft.Json;

namespace Application.PermissionControll
{
    public class DiscordUser
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string ProfPic { get; set; }


        public DiscordUser()  // for json
        {

        }
        public DiscordUser(IUser user)
        {
            Refresh(user);
        }

        public void Refresh(IUser user)
        {
            Id = user.Id;
            Username = user.Username;
            ProfPic = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
        }
    }
}
