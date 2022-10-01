using System.Collections.Generic;
using System.IO;
using Application;
using Application.MinecraftServer.Enums;
using Application.MinecraftServer.EventHandlers;
using Application.MinecraftServer.Util;
using Loggers;

namespace Application.MinecraftServer
{
    /// <summary>
    /// Class holding all the Minecraft Server Instances
    /// </summary>
    public class ServerPark
    {
        /// <summary>
        /// Path of the folder the minecraft servers are stored
        /// </summary>
        public static string ServersFolder { get; } = MinecraftConfig.Instance.MinecraftServersBaseFolder + "Servers\\";
        /// <summary>
        /// Path of the folder which contains the previously deleted servers
        /// </summary>
        public static string DeletedServersFolder { get; } = MinecraftConfig.Instance.MinecraftServersBaseFolder + "Deleted Servers\\";
        /// <summary>
        /// Path of an empty server folder (this is copied into the <see cref="ServersFolder"/> when a new server is created)
        /// </summary>
        public static string EmptyServersFolder { get; } = MinecraftConfig.Instance.MinecraftServersBaseFolder + "Empty Server\\";




        static ServerPark()
        {
            DirectoryInfo info = new(ServersFolder);

            var servers = info.GetDirectories();

            MCServers.Clear();
            foreach (var server in servers)  // loop through all the directories and create the server instances
                RegisterMcServer(server.Name, server.FullName);

            ActiveServerChange = null!;
            ActiveServerPlayerLeft = null!;
            ActiveServerPlayerJoined = null!;
            ActiveServerLogReceived = null!;
            ActiveServerPerformanceMeasured = null!;
            ActiveServerStatusChange = null!;

            ServerAdded = null!;
            ServerDeleted = null!;
            ServerNameChanged = null!;
        }


        /// <summary>
        /// Online minecraft server (only one can be online at a time)
        /// </summary>
        public static IMinecraftServer? ActiveServer { get; private set; }

        /// <summary>
        /// List of all minecraft server instances
        /// </summary>
        public static Dictionary<string, IMinecraftServer> MCServers { get; } = new ();


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

            if (!MCServers.TryGetValue(serverName, out IMinecraftServer? server))
            {
                throw new Exception("Server not found!");
            }

            if (ActiveServer == server)
                return;

            UnSubscribeEventTrackers(ActiveServer);
            ActiveServer = server;
            SubscribeEventTrackers(ActiveServer);


            InvokeActiveServerChanged(ActiveServer);
        }

        /// <summary>
        /// Start the active server.
        /// </summary>
        /// <param name="username">username who initiates the start</param>
        public static void StartServer(string serverName, string username)
        {
            ValidateMaxStorage();

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
        /// Toggles a server, e.g. it starts if it's offline, and stops if it's online.
        /// </summary>
        /// <param name="serverName">server to toggle</param>
        /// <param name="username">username who initiated this action</param>
        public static void ToggleServer(string serverName, string username = "Admin")
        {
            SetActiveServer(serverName);

            if(ActiveServer!.IsRunning)
                ActiveServer.Start(username);
            else
                ActiveServer.Shutdown(username);
        }


        /// <summary>
        /// Creates a new server folder by copying the empty folder to the servers folder.
        /// </summary>
        /// <param name="name">name of the new </param>
        /// <exception cref="Exception"></exception>
        public static IMinecraftServer CreateServer(string name)
        {
            ValidateNameLength(name);

            if (ServerNameExist(name))
                throw new Exception($"The name {name} is already taken");

            ValidateMaxStorage();

            
            string destDir = ServersFolder + name;
            Directory.CreateDirectory(destDir);

            FileHelper.CopyDirectory(EmptyServersFolder, destDir);

            return RegisterMcServer(name, destDir);
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

            MCServers.Remove(oldName, out IMinecraftServer? server);

            if (server == null)
                return;


            MCServers.Add(newName, server);
            server.ServerName = newName;

            InvokeServerNameChange(oldName, newName);
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

            MCServers.Remove(name, out IMinecraftServer? server);
            
            if(server != null)
                InvokeServerDeleted(server);
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

        private static void ValidateMaxStorage()
        {
            var dir = MinecraftConfig.Instance.MinecraftServersBaseFolder;
            long maxDiskSpaceByte = (long)MinecraftConfig.Instance.MaxSumOfDiskSpaceGB * (1024 * 1024 * 1024);

            (bool overflow, long measured) = FileHelper.CheckStorageOverflow(dir, maxDiskSpaceByte);

            string measuredString = FileHelper.StorageFormatter(measured);

            LogService.GetService<MinecraftLogger>().Log("serverpark", "Storage measured: " + measuredString);

            if (overflow)
            {
                LogService.GetService<MinecraftLogger>().Log("serverpark", "Storage OVERFLOW", ConsoleColor.Red);
                throw new Exception($"Disk space full. Max disk space allocated: {MinecraftConfig.Instance.MaxSumOfDiskSpaceGB} GB." +
                    $" Current storage: {measuredString}.");
            }
                
        }
        
        
        /// <summary>
        /// Register a new minecraft server object to the program.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="folderPath"></param>
        private static IMinecraftServer RegisterMcServer(string serverName, string folderPath)
        {
            IMinecraftServer mcServer = new MinecraftServer(serverName, folderPath);
            MCServers.Add(serverName, mcServer);
            InvokeServerAdded(mcServer);

            return mcServer;
        }



        /// <summary>
        /// Event fired when the active server has changed.
        /// </summary>
        public static event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange;

        /// <summary>
        /// Event fired when the active server's status has changed.
        /// </summary>
        public static event EventHandler<ValueEventArgs<ServerStatus>> ActiveServerStatusChange;

        /// <summary>
        /// Event fired when a log message has been received from the active server.
        /// </summary>
        public static event EventHandler<ValueEventArgs<LogMessage>> ActiveServerLogReceived;

        /// <summary>
        /// Event fired when a player joins the active server.
        /// </summary>
        public static event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerJoined;

        /// <summary>
        /// Event fired when a player leaves the active server.
        /// </summary>
        public static event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerLeft;

        /// <summary>
        /// Event fired when a performance measurement data has been received.
        /// </summary>
        public static event EventHandler<ValueEventArgs<(string CPU, string Memory)>> ActiveServerPerformanceMeasured;

        /// <summary>
        /// Event fired when a server's name is changed.
        /// </summary>
        public static event EventHandler<ValueChangedEventArgs<string>> ServerNameChanged;

        /// <summary>
        /// Event fired when a server is added to the ServerPark.
        /// </summary>
        public static event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded;

        /// <summary>
        /// Event fired when a server is deleted from the ServerPark.
        /// </summary>
        public static event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted;




        /// <summary>
        /// Subscribe ServerPark events on a specific minecraft server.
        /// </summary>
        /// <param name="server">server to subscribe</param>
        private static void SubscribeEventTrackers(IMinecraftServer server)
        {
            server.StatusChange += InvokeStatusTracker;
            server.LogReceived += InvokeLogReceived;
            server.PlayerLeft += InvokePlayerLeft;
            server.PlayerJoined += InvokePlayerJoined;
            server.PerformanceMeasured += InvokePerformanceMeasured;
        }

        /// <summary>
        /// Unsubscribe ServerPark events from a minecraft server.
        /// </summary>
        /// <param name="server">server to unsubscribe from</param>
        private static void UnSubscribeEventTrackers(IMinecraftServer? server)
        {
            if (server == null)
                return;

            server.StatusChange -= InvokeStatusTracker;
            server.LogReceived -= InvokeLogReceived;
            server.PlayerLeft -= InvokePlayerLeft;
            server.PlayerJoined -= InvokePlayerJoined;
            server.PerformanceMeasured -= InvokePerformanceMeasured;
        }


        // IMinecraft events
        private static void InvokePerformanceMeasured(object? sender, (string CPU, string Memory) e) =>
            ActiveServerPerformanceMeasured?.Invoke(sender, new (e));
        private static void InvokePlayerJoined(object? sender, MinecraftPlayer e) =>
            ActiveServerPlayerJoined?.Invoke(sender, new (e));
        private static void InvokePlayerLeft(object? sender, MinecraftPlayer e) =>
            ActiveServerPlayerLeft?.Invoke(sender, new (e));
        private static void InvokeLogReceived(object? sender, LogMessage e) =>
            ActiveServerLogReceived?.Invoke(sender, new (e));
        private static void InvokeStatusTracker(object? sender, ServerStatus e) =>
            ActiveServerStatusChange?.Invoke(sender, new(e));
            

        // ServerPark events
        private static void InvokeActiveServerChanged(IMinecraftServer activeServer) =>
            ActiveServerChange?.Invoke(typeof(ServerPark), new(activeServer));
        private static void InvokeServerNameChange(string oldName, string newName) =>
            ServerNameChanged?.Invoke(typeof(ServerPark), new (oldName, newName));
        private static void InvokeServerAdded(IMinecraftServer addedServer) =>
            ServerAdded?.Invoke(typeof(ServerPark), new (addedServer));
        private static void InvokeServerDeleted(IMinecraftServer deletedServer) =>
            ServerDeleted?.Invoke(typeof(ServerPark), new (deletedServer));
    }
}
