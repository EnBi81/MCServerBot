using Loggers;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace HamachiHelper
{
    /// <summary>
    /// Tools for networking part
    /// </summary>
    public static class NetworkingTools
    {

        /// <summary>
        /// 
        /// </summary>
        public static bool CheckNetworking()
        {
            _ = GetLocalIp();
            return CheckForInternetConnection();
        }

        /// <summary>
        /// Gets the local ip address if found
        /// </summary>
        /// <returns>the local ip address if found, else null</returns>
        public static string GetLocalIp()
        {
            LogService.GetService<NetworkLogger>().Log("Getting Local Ip");

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString() != HamachiClient.Address)
                {
                    LogService.GetService<NetworkLogger>().Log("Local Ip Found: " + ip.ToString());
                    return ip.ToString();
                }
            }

            LogService.GetService<NetworkLogger>().LogFatal("No network adapters with an IPv4 address in the system!");

            return null;
        }

        /// <summary>
        /// Checks if the computer is connected to the internet
        /// </summary>
        /// <returns>true if the computer is connected to the internet, else false</returns>
        public static bool CheckForInternetConnection()
        {
            LogService.GetService<NetworkLogger>().Log("Checking internet connection.");

            try
            {
                string url = CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                         "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => //China
                         "http://www.baidu.com",
                    _ => "http://www.gstatic.com/generate_204"
                };

                var client = new HttpClient();
                client.BaseAddress = new Uri(url);
                
                using var response = client.Send(new HttpRequestMessage());
                LogService.GetService<NetworkLogger>().Log("Connected to Internet");

                return true;
            }
            catch 
            {

            }

            LogService.GetService<NetworkLogger>().Log("No Internet Connection.");
            return false;
        }
    }
}
