using Loggers;
using System.Text.RegularExpressions;

namespace HamachiCli
{
    /// <summary>
    /// Retrieves information from the Hamachi client.
    /// </summary>
    public static class HamachiClient
    {
        public static void Setup(string hamachiWorkingDir) => 
            HamachiProcess.Setup(hamachiWorkingDir);


        private static HamachiLogger _logger = LogService.GetService<HamachiLogger>();

        private static string? _address = null;
        /// <summary>
        /// Gets the address of the current computer.
        /// </summary>
        public static string Address { get => _address ??= GetStatus().Address; }


        
        /// <summary>
        /// Gets the status of the current Hamachi client.
        /// </summary>
        /// <returns>A <see cref="HamachiStatus"/> object containing the information.</returns>
        public static HamachiStatus GetStatus()
        {
            _logger.Log("Getting Hamachi Status");

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

            HamachiStatus hamachiStatus = new ()
            {
                Online = status?.Equals("logged in") ?? false,
                Address = address ?? "Unknown",
                NickName = nickname ?? "Unknown",
            };

            _logger.Log($"Returning Hamachi Status: (online: {hamachiStatus.Online}," +
                $" address: {hamachiStatus.Address}, nickname: {hamachiStatus.NickName})");

            return hamachiStatus;
        }

        /// <summary>
        /// Makes Hamachi log in to the Hamachi network.
        /// </summary>
        /// <returns>the text hamachi returned.</returns>
        public static string LogOn()
        {
            _logger.Log("Logging on");
            var data = HamachiProcess.RequestData("logon");
            _logger.Log(data[0]);
            return data[0];
        }

        /// <summary>
        /// Logs out from the Hamachi network.
        /// </summary>
        /// <returns>the text hamachi returned.</returns>
        public static string LogOff()
        {
            _logger.Log("Logging off");
            var data = HamachiProcess.RequestData("logoff");
            _logger.Log(data[0]);
            return data[0];
        }
    }
}
