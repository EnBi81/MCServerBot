﻿using Application.Minecraft.Enums;
using Application.Minecraft.EventHandlers;
using Application.Minecraft.MinecraftServers;
using DataStorage.DataObjects;

namespace Application.Minecraft
{
    /// <summary>
    /// A cozy place to manage the minecraft servers
    /// </summary>
    public interface IServerPark
    {
        public static IServerPark Instance { get; } = new ServerParkProxy();



        /// <summary>
        /// Online minecraft server (only one can be online at a time)
        /// </summary>
        public IMinecraftServer? ActiveServer { get; }

        /// <summary>
        /// Readonly collection of the minecraft servers
        /// </summary>
        public  IReadOnlyDictionary<string, IMinecraftServer> MCServers { get; } 


        /// <summary>
        /// Start the active server.
        /// </summary>
        /// <param name="user">user who initiated the start</param>
        public Task StartServer(string serverName, DataUser user);

        /// <summary>
        /// Stop the active server.
        /// </summary>
        /// <param name="user">user who initiated the stop</param>
        public Task StopActiveServer(DataUser user);


        /// <summary>
        /// Toggles a server, e.g. it starts if it's offline, and stops if it's online.
        /// </summary>
        /// <param name="serverName">server to toggle</param>
        /// <param name="user">user who initiated this action</param>
        public Task ToggleServer(string serverName, DataUser user);


        /// <summary>
        /// Creates a new server folder by copying the empty folder to the servers folder.
        /// </summary>
        /// <param name="name">name of the new </param>
        /// <exception cref="Exception"></exception>
        public Task<IMinecraftServer> CreateServer(string name, DataUser user);

        /// <summary>
        /// Changes an already existing minecraft server's name if it is not running
        /// </summary>
        /// <param name="oldName">Name of the server to change</param>
        /// <param name="newName">New name of the server</param>
        /// <exception cref="Exception">If the name has invalid length</exception>
        /// <exception cref="Exception">If the new name is already taken</exception>
        /// <exception cref="Exception">If the server to change is running</exception>
        public Task RenameServer(string oldName, string newName, DataUser user);

        /// <summary>
        /// Deletes a server by moving to the <see cref="DeletedServersFolder"/>.
        /// </summary>
        /// <param name="name">Server to be moved.</param>
        /// <exception cref="Exception">If the server does not exist, or it's running.</exception>
        public Task DeleteServer(string name, DataUser user);



        /// <summary>
        /// Event fired when the active server has changed.
        /// </summary>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange;

        /// <summary>
        /// Event fired when the active server's status has changed.
        /// </summary>
        public event EventHandler<ValueEventArgs<ServerStatus>> ActiveServerStatusChange;

        /// <summary>
        /// Event fired when a log message has been received from the active server.
        /// </summary>
        public event EventHandler<ValueEventArgs<LogMessage>> ActiveServerLogReceived;

        /// <summary>
        /// Event fired when a player joins the active server.
        /// </summary>
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerJoined;

        /// <summary>
        /// Event fired when a player leaves the active server.
        /// </summary>
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerLeft;

        /// <summary>
        /// Event fired when a performance measurement data has been received.
        /// </summary>
        public event EventHandler<ValueEventArgs<(string CPU, string Memory)>> ActiveServerPerformanceMeasured;

        /// <summary>
        /// Event fired when a server's name is changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<string>> ServerNameChanged;

        /// <summary>
        /// Event fired when a server is added to the ServerPark.
        /// </summary>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded;

        /// <summary>
        /// Event fired when a server is deleted from the ServerPark.
        /// </summary>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted;
    }
}