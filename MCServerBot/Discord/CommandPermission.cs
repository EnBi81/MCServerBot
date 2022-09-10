using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCWebServer.Discord
{
    internal class CommandPermission
    {
        private static readonly HashSet<ulong> _permissions = ReadPermissions();

        public static bool HasPermission(ulong id)
        {
            return _permissions.Contains(id);
        }

        public static void GrantPermission(ulong id)
        {
            if (HasPermission(id))
                throw new Exception($"User {id} already has permission");

            _permissions.Add(id);

            SavePermission();
        }

        public static void RevokePermission(ulong id)
        {
            if (!HasPermission(id))
                throw new Exception($"User {id} does not have permission to revoke");

            _permissions.Remove(id);

            SavePermission();
        }



        private static string PermissionFile => "user-permissions.json";
        private static HashSet<ulong> ReadPermissions()
        {
            try
            {
                string text = File.ReadAllText(PermissionFile);
                return JsonConvert.DeserializeObject<HashSet<ulong>>(text) ?? new HashSet<ulong>();
            }catch (Exception)
            {
                return new HashSet<ulong>();
            }
        }

        private static void SavePermission() 
        {
            string text = JsonConvert.SerializeObject(_permissions);
            File.WriteAllText(PermissionFile, text);
        }
    }
}
