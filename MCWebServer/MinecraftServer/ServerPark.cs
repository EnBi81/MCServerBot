using System;
using System.Collections.Generic;
using System.IO;
using MCWebServer.MinecraftServer.Enums;
using MCWebServer.MinecraftServer.Util;

namespace MCWebServer.MinecraftServer
{
    /// <summary>
    /// Class holding all the Minecraft Server Instances
    /// </summary>
    public class ServerPark
    {
        /// <summary>
        /// Path of the folder the minecraft servers are stored
        /// </summary>
        public static string ServersFolder { get; } = Config.Config.Instance.MinecraftServersBaseFolder + "Servers\\";
        /// <summary>
        /// Path of the folder which contains the previously deleted servers
        /// </summary>
        public static string DeletedServersFolder { get; } = Config.Config.Instance.MinecraftServersBaseFolder + "Deleted Servers\\";
        /// <summary>
        /// Path of an empty server folder (this is copied into the <see cref="ServersFolder"/> when a new server is created)
        /// </summary>
        public static string EmptyServersFolder { get; } = Config.Config.Instance.MinecraftServersBaseFolder + "Empty Server\\";


        static ServerPark()
        {
            DirectoryInfo info = new(ServersFolder);

            var servers = info.GetDirectories();

            MCServers.Clear();
            foreach (var server in servers)  // loop through all the directories and create the server instances
                RegisterMcServer(server.Name, server.FullName);
        }


        /// <summary>
        /// Online minecraft server (only one can be online at a time)
        /// </summary>
        public static IMinecraftServer ActiveServer { get; private set; }

        /// <summary>
        /// List of all minecraft server instances
        /// </summary>
        public static Dictionary<string, IMinecraftServer> MCServers { get; } = new ();

        public static IMinecraftServer Keklepcso { get; } 


        /// <summary>
        /// Set a server as active.
        /// </summary>
        /// <param name="server">Server to set as active</param>
        /// <exception cref="Exception">If another server is already running.</exception>
        private static void SetActiveServer(string serverName)
        {
            if(ActiveServer != null)
            {
                if(ActiveServer.IsRunning)
                    throw new Exception("Another Server is Running Already!");
            }

            if (!MCServers.TryGetValue(serverName, out IMinecraftServer server))
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
        /// Creates a new server folder by copying the empty folder to the servers folder.
        /// </summary>
        /// <param name="name">name of the new </param>
        /// <exception cref="Exception"></exception>
        public static void CreateServer(string name)
        {
            ValidateNameLength(name);

            if (ServerNameExist(name))
                throw new Exception($"The name {name} is already taken");

            
            string destDir = ServersFolder + name;
            Directory.CreateDirectory(destDir);

            FileHelper.CopyDirectory(EmptyServersFolder, destDir);

            RegisterMcServer(name, destDir);
        }

        /// <summary>
        /// Changes an already existing minecraft server's name if it is not running
        /// </summary>
        /// <param name="oldName">Name of the server to change</param>
        /// <param name="newName">New name of the server</param>
        /// <exception cref="Exception">If the name has invalid length</exception>
        /// <exception cref="Exception">If the new name is already taken</exception>
        /// <exception cref="Exception">If the server to change is running</exception>
        public static void RenameServer(string oldName, string newName)
        {
            ValidateNameLength(newName);

            if (!ServerNameExist(oldName))
                throw new Exception($"The server '{oldName}' does not exist.");

            if (ServerNameExist(newName))
                throw new Exception($"The name '{newName}' is already taken");

            if (ActiveServer != null && ActiveServer.ServerName == oldName && ActiveServer.IsRunning)
                throw new Exception($"{oldName} is Runnning! Please stop the server first.");

            FileHelper.MoveDirectory(ServersFolder + oldName, ServersFolder + newName);

            MCServers.Remove(oldName, out IMinecraftServer server);
            MCServers.Add(newName, server);
            server.ServerName = newName;
        }

        /// <summary>
        /// Deletes a server by moving to the <see cref="DeletedServersFolder"/>.
        /// </summary>
        /// <param name="name">Server to be moved.</param>
        /// <exception cref="Exception">If the server does not exist, or it's running.</exception>
        public static void DeleteServer(string name)
        {
            if (!ServerNameExist(name))
                throw new Exception($"The server '{name}' does not exist.");

            if (ActiveServer != null && ActiveServer.ServerName == name && ActiveServer.IsRunning)
                throw new Exception($"To delete this server, first make sure it is stopped.");

            string newDir = DeletedServersFolder + name + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            FileHelper.MoveDirectory(ServersFolder + name, newDir);

            MCServers.Remove(name);
        }

        /// <summary>
        /// Check if the name already exists.
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>true if it exists, else false.</returns>
        private static bool ServerNameExist(string name) => MCServers.ContainsKey(name);

        /// <summary>
        /// Checks if the name's length is valid, throws exception if yes.
        /// </summary>
        /// <param name="name">name to check</param>
        /// <exception cref="Exception">if the name is not valid.</exception>
        private static void ValidateNameLength(string name)
        {
            if(!(name.Length <= IMinecraftServer.NAME_MAX_LENGTH && name.Length >= IMinecraftServer.NAME_MIN_LENGTH))
                throw new Exception($"Name must be no longer than {IMinecraftServer.NAME_MAX_LENGTH} characters and more than {IMinecraftServer.NAME_MIN_LENGTH}!");
        }
        
        
        /// <summary>
        /// Register a new minecraft server object to the program.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="folderPath"></param>
        private static void RegisterMcServer(string serverName, string folderPath)
        {
            IMinecraftServer mcServer = new MinecraftServer(serverName, folderPath);
            MCServers.Add(serverName, mcServer);
        }



        /// <summary>
        /// Event fired when the active server has changed.
        /// </summary>
        public static event EventHandler<IMinecraftServer> ActiveServerChange;

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




        /// <summary>
        /// Subscribe ServerPark events on a specific minecraft server.
        /// </summary>
        /// <param name="server">server to subscribe</param>
        private static void SubscribeEventTrackers(IMinecraftServer server)
        {
            server.StatusChange += StatusTracker;
            server.LogReceived += LogReceived;
            server.PlayerLeft += PlayerLeft;
            server.PlayerJoined += PlayerJoined;
            server.PerformanceMeasured += PerformanceMeasured;
        }

        /// <summary>
        /// Unsubscribe ServerPark events from a minecraft server.
        /// </summary>
        /// <param name="server">server to unsubscribe from</param>
        private static void UnSubscribeEventTrackers(IMinecraftServer server)
        {
            if (server == null)
                return;

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
