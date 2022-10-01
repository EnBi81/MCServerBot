using DiscordBot;
using DiscordBot.PermissionControll;
using HamachiHelper;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Application.PermissionControll
{
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
            WebsiteDomainUrl  = protocol + "keklepcso.com"                      + port;
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
        private static byte[] GetHash(string inputString)
        {
            using HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static DiscordUser GetUser(string code)
        {
            if (!_permissions.ContainsKey(code))
                return null;

            return BotPermission.GetUser(_permissions[code]);
        }

        public static bool HasAccess(string code)
        {
            return _permissions.ContainsKey(code);
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

        private static void SavePermission()
        {
            string text = JsonConvert.SerializeObject(_permissions);
            File.WriteAllText(PermissionFile, text);
        }
    }
}
