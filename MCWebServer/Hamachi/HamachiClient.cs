using MCWebServer.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCWebServer.Hamachi
{
    internal static class HamachiClient
    {
        private static string _address = null;
        public static string Address { get => _address ??= GetStatus().Address; }


        

        public static HamachiStatus GetStatus()
        {
            LogService.GetService<HamachiLogger>().Log("Getting Hamachi Status");

            var data = HamachiProcess.RequestData(null);

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

            LogService.GetService<HamachiLogger>().Log($"Returning Hamachi Status: (online: {hamachiStatus.Online}," +
                $" address: {hamachiStatus.Address}, nickname: {hamachiStatus.NickName})");

            return hamachiStatus;
        }

        public static string LogOn()
        {
            LogService.GetService<HamachiLogger>().Log("Logging on");
            var data = HamachiProcess.RequestData("logon");
            LogService.GetService<HamachiLogger>().Log(data[0]);
            return data[0];
        }

        public static string LogOff()
        {
            LogService.GetService<HamachiLogger>().Log("Logging off");
            var data = HamachiProcess.RequestData("logoff");
            LogService.GetService<HamachiLogger>().Log(data[0]);
            return data[0];
        }
    }
}
