using Loggers;
using Loggers.Loggers;
using Shared.Exceptions;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace HamachiCli
{
    /// <summary>
    /// Tools for networking part
    /// </summary>
    public static class NetworkingTools
    {
        private static NetworkLogger _logger = LogService.GetService<NetworkLogger>();


        /// <summary>
        /// Checks if the networking is okay.
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
            _logger.Log("Getting Local Ip");

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString() != HamachiClient.Address)
                {
                    _logger.Log("Local Ip Found: " + ip.ToString());
                    return ip.ToString();
                }
            }

            throw new MCInternalException("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// Checks if the computer is connected to the internet
        /// </summary>
        /// <returns>true if the computer is connected to the internet, else false</returns>
        public static bool CheckForInternetConnection()
        {
            _logger.Log("Checking internet connection.");

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
                _logger.Log("Connected to Internet");

                return true;
            }
            catch 
            {

            }

            _logger.Log("No Internet Connection.");
            return false;
        }
    }
}
