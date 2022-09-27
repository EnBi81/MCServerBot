using Discord;
using Newtonsoft.Json;

namespace Application.PermissionControll
{
    public class DiscordUser
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string ProfPic { get; set; }
        public DateTime LastUpdated { get; set; }

        [JsonIgnore]
        private IUser _user;


        public DiscordUser()  // for json
        {

        }
        public DiscordUser(IUser user)
        {
            _user = user;
            Id = user.Id;
            Username = user.Username;
            ProfPic = _user.GetAvatarUrl() ?? _user.GetDefaultAvatarUrl();
            LastUpdated = DateTime.Now;
        }



        public async Task<string> GetProfilePicUrl()
        {
            if((DateTime.Now - LastUpdated).TotalDays > 1)
            {
                try
                {
                    LastUpdated = DateTime.Now;

                    if (_user == null)
                        _user = await DiscordBot.Discord.DiscordBot.Bot.SocketClient.GetUserAsync(Id);

                    ProfPic = _user.GetAvatarUrl() ?? _user.GetDefaultAvatarUrl();
                    
                }
                catch { }
            }
            

            return ProfPic;
        }
    }
}
