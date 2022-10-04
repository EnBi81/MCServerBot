using Discord;
using Application.PermissionControll;
using Newtonsoft.Json;

namespace DiscordBot.PermissionControll
{
    /// <summary>
    /// Handles Discord permissions given by the bot.
    /// </summary>
    public static class BotPermission
    {
        private static readonly Dictionary<ulong, DiscordUser> _permissions = ReadPermissions();

        /// <summary>
        /// Gets if the discord user has permission
        /// </summary>
        /// <param name="id">discord user's id</param>
        /// <returns>if the user has permission.</returns>
        public static bool HasPermission(ulong id)
        {
            return _permissions.ContainsKey(id);
        }

        /// <summary>
        /// Grants permission to a user.
        /// </summary>
        /// <param name="user">user to give permission to.</param>
        /// <exception cref="Exception"></exception>
        public static void GrantPermission(IUser user)
        {
            ulong id = user.Id;

            if (HasPermission(id))
                throw new Exception($"User {id} already has permission");

            _permissions.Add(user.Id, new DiscordUser(user));

            SavePermission();
        }

        /// <summary>
        /// Revokes the permission for a discord user.
        /// </summary>
        /// <param name="id">the user's id</param>
        /// <exception cref="Exception"></exception>
        public static void RevokePermission(ulong id)
        {
            if (!HasPermission(id))
                throw new Exception($"User {id} does not have permission to revoke");

            _permissions.Remove(id);
            SavePermission();

            PermissionRemoved?.Invoke(new object(), id);
        }

        /// <summary>
        /// Gets the discord user if it has permission
        /// </summary>
        /// <param name="id">id of the user</param>
        /// <returns></returns>
        public static DiscordUser? GetUser(ulong id)
        {
            if (!_permissions.ContainsKey(id))
                return null;

            return _permissions[id];
        }


        public static async Task<DiscordUser?> RefreshUser(ulong id)
        {
            var user = GetUser(id);

            if (user == null)
                return null;

            IUser discordUser = await Discord.DiscordBot.Bot.SocketClient.GetUserAsync(id);
            user.Refresh(discordUser);

            return user;
        }

        public static event EventHandler<ulong> PermissionRemoved;






        private static string PermissionFile => "Resources/user-permissions.json";
        private static Dictionary<ulong, DiscordUser> ReadPermissions()
        {
            Dictionary<ulong, DiscordUser>? data = null;

            try
            {
                string text = File.ReadAllText(PermissionFile);
                data = JsonConvert.DeserializeObject<Dictionary<ulong, DiscordUser>>(text);
            }
            catch {}

            return data ?? new Dictionary<ulong, DiscordUser>();
        }

        private static void SavePermission()
        {
            string text = JsonConvert.SerializeObject(_permissions);
            File.WriteAllText(PermissionFile, text);
        }
    }
}
