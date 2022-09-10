using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCWebServer.Hamachi
{
    internal static class HamachiClient
    {
        public static HamachiStatus GetStatus()
        {
            var data = HamachiProcess.requestData(null);

            string? status = null;
            string? address = null;
            string? nickname = null;

            foreach (var item in data)
            {
                if(new Regex("^\\s*status\\s*:\\s*.*").IsMatch(item))
                    status = item[(item.IndexOf(":") + 2)..];
                if (new Regex("^\\s*nickname\\s*:\\s*.*").IsMatch(item))
                    nickname = item[(item.IndexOf(":") + 2)..];
                if (new Regex("^\\s*address\\s*:\\s*.*").IsMatch(item))
                {
                    address = item[(item.IndexOf(":") + 2)..];
                    address = address[..address.IndexOf(" ")];
                }
            }

            HamachiStatus hamachiStatus = new HamachiStatus()
            {
                Online = status?.Equals("logged in") ?? false,
                Address = address ?? "Unknown",
                NickName = nickname ?? "Unknown",
            };

            return hamachiStatus;
        }

        public static string LogOn()
        {
            var data = HamachiProcess.requestData("logon");
            return data[0];
        }

        public static string LogOff()
        {
            var data = HamachiProcess.requestData("logoff");
            return data[0];
        }
    }
}
