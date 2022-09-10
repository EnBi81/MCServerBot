using System;
using System.Collections.Generic;
using System.IO;

namespace MCWebServer.MinecraftServer
{
    /// <summary>
    /// Class holding all the Minecraft Server Instances
    /// </summary>
    public class ServerPark
    {
        static ServerPark()
        {
            var computerAddress = Hamachi.HamachiClient.GetStatus().Address; //get hamachi address

            string serversFolderName = Config.Config.Instance.MinecraftServersFolder;
            DirectoryInfo info = new(serversFolderName);

            var servers = info.GetDirectories();
            foreach (var server in servers)  // loop through all the directories and create the server instances
            {
                string serverName = server.Name;
                string folderPath = server.FullName;

                MinecraftServer mcServer = new(serverName, computerAddress, folderPath, Config.Config.Instance.JavaLocation);
                MCServers.Add(serverName, mcServer);
            }
        }


        /// <summary>
        /// Online minecraft server (only one can be online at a time)
        /// </summary>
        public static MinecraftServer ActiveServer { get; private set; }

        /// <summary>
        /// List of all minecraft server instances
        /// </summary>
        public static Dictionary<string, MinecraftServer> MCServers { get; } = new ();

        public static MinecraftServer Keklepcso { get; } 


        /// <summary>
        /// Set a server as active.
        /// </summary>
        /// <param name="server">Server to set as active</param>
        /// <exception cref="Exception">If another server is already running.</exception>
        private static void SetActiveServer(string serverName)
        {
            if(ActiveServer != null && !ActiveServer.IsRunning())
            {
                throw new Exception("Another Server is Running Already!");
            }

            if (!MCServers.TryGetValue(serverName, out MinecraftServer server))
            {
                throw new Exception("Server not found!");
            }

            if (ActiveServer == server)
                return;

            UnSubscribeEventTrackers(ActiveServer);
            ActiveServer = server;
            SubscribeEventTrackers(ActiveServer);

            
            ActiveServerChange?.Invoke(null, ActiveServer);
        }

        /// <summary>
        /// Start the active server.
        /// </summary>
        /// <param name="username">username who initiates the start</param>
        public static void StartServer(string serverName, string username)
        {
            SetActiveServer(serverName);
            ActiveServer?.Start(username);
        }

        /// <summary>
        /// Stop the active server.
        /// </summary>
        /// <param name="username">username who initiates the stop</param>
        public static void StopActiveServer(string username)
        {
            ActiveServer?.Shutdown(username);
        }

        /// <summary>
        /// Event fired when the active server has changed.
        /// </summary>
        public static event EventHandler<MinecraftServer> ActiveServerChange;

        /// <summary>
        /// Event fired when the active server's status has changed.
        /// </summary>
        public static event EventHandler<ServerStatus> ActiveServerStatusChange;

        /// <summary>
        /// Event fired when a log message has been received from the active server.
        /// </summary>
        public static event EventHandler<LogMessage> ActiveServerLogReceived;

        /// <summary>
        /// Event fired when a player joins the active server.
        /// </summary>
        public static event EventHandler<MinecraftPlayer> ActiveServerPlayerJoined;

        /// <summary>
        /// Event fired when a player leaves the active server.
        /// </summary>
        public static event EventHandler<MinecraftPlayer> ActiveServerPlayerLeft;

        /// <summary>
        /// Event fired when a performance measurement data has been received.
        /// </summary>
        public static event EventHandler<(string CPU, string Memory)> ActiveServerPerformanceMeasured;




        
        private static void SubscribeEventTrackers(MinecraftServer server)
        {
            server.StatusChange += StatusTracker;
            server.LogReceived += LogReceived;
            server.PlayerLeft += PlayerLeft;
            server.PlayerJoined += PlayerJoined;
            server.PerformanceMeasured += PerformanceMeasured;
            
        }

        private static void UnSubscribeEventTrackers(MinecraftServer server)
        {
            server.StatusChange -= StatusTracker;
            server.LogReceived -= LogReceived;
            server.PlayerLeft -= PlayerLeft;
            server.PlayerJoined -= PlayerJoined;
            server.PerformanceMeasured -= PerformanceMeasured;
        }



        private static void PerformanceMeasured(object sender, (string CPU, string Memory) e) =>
            ActiveServerPerformanceMeasured?.Invoke(sender, e);
        private static void PlayerJoined(object sender, MinecraftPlayer e) =>
            ActiveServerPlayerJoined?.Invoke(sender, e);
        private static void PlayerLeft(object sender, MinecraftPlayer e) =>
            ActiveServerPlayerLeft?.Invoke(sender, e);
        private static void LogReceived(object sender, LogMessage e) =>
            ActiveServerLogReceived?.Invoke(sender, e);
        private static void StatusTracker(object sender, ServerStatus e) => 
            ActiveServerStatusChange?.Invoke(sender, e);
    }
}
