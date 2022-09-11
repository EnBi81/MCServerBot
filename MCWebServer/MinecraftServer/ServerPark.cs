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
        public static string ServersFolder { get; } = Config.Config.Instance.MinecraftServersBaseFolder + "Servers\\";
        public static string DeletedServersFolder { get; } = Config.Config.Instance.MinecraftServersBaseFolder + "Deleted Servers\\";
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
            if(ActiveServer != null)
            {
                if(!ActiveServer.IsRunning())
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
        /// Creates a new server folder by copying the empty folder to the servers folder.
        /// </summary>
        /// <param name="name">name of the new </param>
        /// <exception cref="Exception"></exception>
        public static void CreateServer(string name)
        {
            if (!ValidateNameLength(name))
                throw new Exception("Name must be less than 40 characters and more than 3!");

            if (ServerNameExist(name))
                throw new Exception($"The name {name} is already taken");

            
            string destDir = ServersFolder + name;
            Directory.CreateDirectory(destDir);

            FileHelper.CopyDirectory(EmptyServersFolder, destDir);

            RegisterMcServer(name, destDir);
        }

        public static void RenameServer(string oldName, string newName)
        {
            if (!ValidateNameLength(newName))
                throw new Exception("Name must be less than 40 characters and more than !");

            if (!ServerNameExist(oldName))
                throw new Exception($"The server '{oldName}' does not exist.");

            if (ServerNameExist(newName))
                throw new Exception($"The name {newName} is already taken");

            if (ActiveServer != null && ActiveServer.ServerName == oldName && ActiveServer.IsRunning())
                throw new Exception($"To rename this server, first make sure it is stopped.");

            FileHelper.MoveDirectory(ServersFolder + oldName, ServersFolder + newName);

            MCServers.Remove(oldName, out MinecraftServer server);
            MCServers.Add(newName, server);
            server.ServerName = newName;
        }

        public static void DeleteServer(string name)
        {
            if (!ServerNameExist(name))
                throw new Exception($"The server '{name}' does not exist.");

            if (ActiveServer != null && ActiveServer.ServerName == name && ActiveServer.IsRunning())
                throw new Exception($"To delete this server, first make sure it is stopped.");

            string newDir = DeletedServersFolder + name + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            FileHelper.MoveDirectory(ServersFolder + name, newDir);

            MCServers.Remove(name);
        }

        private static bool ServerNameExist(string name) => MCServers.ContainsKey(name);

        private static bool ValidateNameLength(string name) => name.Length <= MinecraftServer.NAME_MAX_LENGTH && name.Length >= MinecraftServer.NAME_MIN_LENGTH;
        
        
        private static void RegisterMcServer(string serverName, string folderPath)
        {
            MinecraftServer mcServer = new(serverName, folderPath);
            MCServers.Add(serverName, mcServer);
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
