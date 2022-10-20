using Application.Minecraft.Enums;
using Application.Minecraft.EventHandlers;
using Application.Minecraft.MinecraftServers;
using DataStorage;
using DataStorage.DataObjects;
using DataStorage.Interfaces;

namespace Application.Minecraft
{
    internal class ServerParkProxy : IServerPark
    {
        private static readonly IServerParkEventRegister _serverParkEventRegister = EventRegisterCollection.ServerParkEventRegister;




        private readonly ServerPark _serverPark;
        

        internal ServerParkProxy()
        {
            _serverPark = new ServerPark();
        }

        /// <inheritdoc/>
        public IMinecraftServer? ActiveServer => _serverPark.ActiveServer;

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, IMinecraftServer> MCServers => _serverPark.MCServers;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange 
        { 
            add => _serverPark.ActiveServerChange += value;
            remove => _serverPark.ActiveServerChange -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<ServerStatus>> ActiveServerStatusChange
        { 
            add => _serverPark.ActiveServerStatusChange += value;
            remove => _serverPark.ActiveServerStatusChange -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<LogMessage>> ActiveServerLogReceived
        {
            add => _serverPark.ActiveServerLogReceived += value;
            remove => _serverPark.ActiveServerLogReceived -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerJoined
        {
            add => _serverPark.ActiveServerPlayerJoined += value;
            remove => _serverPark.ActiveServerPlayerJoined -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerLeft
        {
            add => _serverPark.ActiveServerPlayerLeft += value;
            remove => _serverPark.ActiveServerPlayerLeft -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<(string CPU, string Memory)>> ActiveServerPerformanceMeasured
        {
            add => _serverPark.ActiveServerPerformanceMeasured += value;
            remove => _serverPark.ActiveServerPerformanceMeasured -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<ValueChangedEventArgs<string>> ServerNameChanged
        {
            add => _serverPark.ServerNameChanged += value;
            remove => _serverPark.ServerNameChanged -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded
        {
            add => _serverPark.ServerAdded += value;
            remove => _serverPark.ServerAdded -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted
        {
            add => _serverPark.ServerDeleted += value;
            remove => _serverPark.ServerDeleted -= value;
        }

        /// <inheritdoc/>
        public async Task<IMinecraftServer> CreateServer(string serverName, DataUser user)
        {
            var res = await _serverPark.CreateServer(serverName, user);
            await _serverPark.CreateServer(serverName, user, _serverParkEventRegister.CreateServer);

            return res;
        }

        /// <inheritdoc/>
        public async Task<IMinecraftServer> DeleteServer(string name, DataUser user)
        {
            var server = await _serverPark.DeleteServer(name, user);
            await _serverParkEventRegister.DeleteServer(user.Id, server.Id);

            return server;
        }

        /// <inheritdoc/>
        public async Task<IMinecraftServer> RenameServer(string oldName, string newName, DataUser user)
        {
            var server = await _serverPark.RenameServer(oldName, newName, user);
            await _serverParkEventRegister.RenameServer(user.Id, server.Id, newName);

            return server;
        }

        /// <inheritdoc/>
        public async Task StartServer(string serverName, DataUser user)
        {
            await _serverPark.StartServer(serverName, user);

            var server = ActiveServer;
            await _serverParkEventRegister.StartServer(user.Id, server!.Id);
        }

        /// <inheritdoc/>
        public async Task StopActiveServer(DataUser user)
        {
            await _serverPark.StopActiveServer(user);

            var server = ActiveServer;
            await _serverParkEventRegister.StopServer(user.Id, server!.Id);
        }

        /// <inheritdoc/>
        public async Task ToggleServer(string serverName, DataUser user)
        {
            bool isRunning = ActiveServer?.IsRunning ?? false;

            await _serverPark.ToggleServer(serverName, user);

            var server = ActiveServer;
            if (isRunning)
                await _serverParkEventRegister.StopServer(user.Id, server!.Id);
            else
                await _serverParkEventRegister.StartServer(user.Id, server!.Id);
        }
    }
}
