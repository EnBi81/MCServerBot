using Discord;
using Application.PermissionControll;
using Newtonsoft.Json;

namespace DiscordBot.PermissionControll
{
    public static class BotPermission
    {
        private static readonly Dictionary<ulong, DiscordUser> _permissions = ReadPermissions();

        public static bool HasPermission(ulong id)
        {
            return _permissions.ContainsKey(id);
        }

        public static void GrantPermission(ulong id, IUser user)
        {
            if (HasPermission(id))
                throw new Exception($"User {id} already has permission");

            _permissions.Add(user.Id, new DiscordUser(user));

            SavePermission();
        }

        public static void RevokePermission(ulong id)
        {
            if (!HasPermission(id))
                throw new Exception($"User {id} does not have permission to revoke");

            _permissions.Remove(id);
            SavePermission();

            PermissionRemoved?.Invoke(new object(), id);
        }

        public static DiscordUser GetUser(ulong id)
        {
            if (!_permissions.ContainsKey(id))
                return null;

            return _permissions[id];
        }

        public static event EventHandler<ulong> PermissionRemoved;






        private static string PermissionFile => "user-permissions.json";
        private static Dictionary<ulong, DiscordUser> ReadPermissions()
        {
            try
            {
                string text = File.ReadAllText(PermissionFile);
                return JsonConvert.DeserializeObject<Dictionary<ulong, DiscordUser>>(text) ?? new Dictionary<ulong, DiscordUser>();
            }
            catch (Exception)
            {
                return new Dictionary<ulong, DiscordUser>();
            }
        }

        private static void SavePermission()
        {
            string text = JsonConvert.SerializeObject(_permissions);
            File.WriteAllText(PermissionFile, text);
        }
    }
}
