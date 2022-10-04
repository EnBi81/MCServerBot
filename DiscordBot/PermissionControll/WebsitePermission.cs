using DiscordBot;
using DiscordBot.PermissionControll;
using HamachiHelper;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Application.PermissionControll
{
    /// <summary>
    /// Handles the website permissions.
    /// </summary>
    public static class WebsitePermission
    {
        public static string CookieName => "minecraft-web-login";

        static WebsitePermission()
        {
            BotPermission.PermissionRemoved += DCPermissionRemoved;


            int portNumber = DiscordConfig.Instance.WebsiteHttpsPort;
            string protocol = "https://";
            string port = portNumber == 443 ? string.Empty : ":" + portNumber;

            WebsiteHamachiUrl = protocol + HamachiClient.Address        + port;
            WebsiteDomainUrl  = protocol + "keklepcso.com"              + port;
            WebsiteLocalUrl   = protocol + NetworkingTools.GetLocalIp() + port;
        }

        public static string WebsiteHamachiUrl { get; } 
        public static string WebsiteDomainUrl { get; } 
        public static string WebsiteLocalUrl { get; }




        private static readonly Dictionary<string, ulong> _permissions = ReadPermissions();


        private static void DCPermissionRemoved(object? sender, ulong e)
        {
            foreach (var permission in _permissions)
            {
                if(permission.Value == e)
                {
                    var code = permission.Key;
                    _permissions.Remove(code);
                    PermissionRemoved?.Invoke(new object(), code);
                }
            }

            SavePermission();
        }

        public static event EventHandler<string> PermissionRemoved = null!;

        public static string CreatePrivateUrl(string baseAddress, string code)
        {
            return $"{baseAddress}/?code={code}";
        }

        /// <summary>
        /// Gets the code for a discord user.
        /// </summary>
        /// <param name="id">the discord user's id.</param>
        /// <returns></returns>
        public static string GetCode(ulong id)
        {
            string code;

            if (_permissions.ContainsValue(id))
            {
                code = (from perm in _permissions where perm.Value == id select perm.Key).First();
            }
            else
            {
                code = GetHashString(DateTime.Now.ToString("G") + id.ToString());
            }

            if (!_permissions.ContainsKey(code))
            {
                _permissions.Add(code, id);
                SavePermission();
            }

            return code;
        }

        

        /// <summary>
        /// Gets the user associated with the code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static DiscordUser? GetUser(string code)
        {
            if (!_permissions.ContainsKey(code))
                return null;

            return BotPermission.GetUser(_permissions[code]);
        }

        /// <summary>
        /// Gets if the code is registered and has active access.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool HasAccess(string code)
        {
            return _permissions.ContainsKey(code);
        }


        public static async Task<DiscordUser?> RefreshUser(string code)
        {
            DiscordUser? user = null;

            if(!_permissions.ContainsKey(code))
                throw new Exception("No user found with code " + code);

            ulong discordId = _permissions[code];
            user = await BotPermission.RefreshUser(discordId);

            return user;
        }





        /// <summary>
        /// Gets the hash of a string as a byte array.
        /// </summary>
        /// <param name="inputString">string to hash</param>
        /// <returns></returns>
        private static byte[] GetHash(string inputString)
        {
            using HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        /// <summary>
        /// Gets the hash of a string as a string.
        /// </summary>
        /// <param name="inputString">string to hash</param>
        /// <returns></returns>
        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new ();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }


        private static string PermissionFile => "Resources/web-permissions.json";
        private static Dictionary<string, ulong> ReadPermissions()
        {
            try
            {
                string text = File.ReadAllText(PermissionFile);
                return JsonConvert.DeserializeObject<Dictionary<string, ulong>>(text) ?? new Dictionary<string, ulong>();
            }
            catch (Exception)
            {
                return new Dictionary<string, ulong>();
            }
        }

        /// <summary>
        /// Saves the permissions to a file.
        /// </summary>
        private static void SavePermission()
        {
            string text = JsonConvert.SerializeObject(_permissions);
            File.WriteAllText(PermissionFile, text);
        }
    }
}
