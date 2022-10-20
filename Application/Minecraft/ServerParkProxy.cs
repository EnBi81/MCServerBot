using Application.Minecraft.Enums;
using Application.Minecraft.EventHandlers;
using Application.Minecraft.MinecraftServers;
using DataStorage.DataObjects;

namespace Application.Minecraft
{
    internal class ServerParkProxy : IServerPark
    {
        private readonly ServerPark _serverPark;

        internal ServerParkProxy()
        {
            _serverPark = new ServerPark();
        }

        public IMinecraftServer? ActiveServer => _serverPark.ActiveServer;

        public IReadOnlyDictionary<string, IMinecraftServer> MCServers => _serverPark.MCServers;

        public event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange 
        { 
            add => _serverPark.ActiveServerChange += value;
            remove => _serverPark.ActiveServerChange -= value;
        }
        public event EventHandler<ValueEventArgs<ServerStatus>> ActiveServerStatusChange
        { 
            add => _serverPark.ActiveServerStatusChange += value;
            remove => _serverPark.ActiveServerStatusChange -= value;
        }
        public event EventHandler<ValueEventArgs<LogMessage>> ActiveServerLogReceived
        {
            add => _serverPark.ActiveServerLogReceived += value;
            remove => _serverPark.ActiveServerLogReceived -= value;
        }
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerJoined
        {
            add => _serverPark.ActiveServerPlayerJoined += value;
            remove => _serverPark.ActiveServerPlayerJoined -= value;
        }
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerLeft
        {
            add => _serverPark.ActiveServerPlayerLeft += value;
            remove => _serverPark.ActiveServerPlayerLeft -= value;
        }
        public event EventHandler<ValueEventArgs<(string CPU, string Memory)>> ActiveServerPerformanceMeasured
        {
            add => _serverPark.ActiveServerPerformanceMeasured += value;
            remove => _serverPark.ActiveServerPerformanceMeasured -= value;
        }
        public event EventHandler<ValueChangedEventArgs<string>> ServerNameChanged
        {
            add => _serverPark.ServerNameChanged += value;
            remove => _serverPark.ServerNameChanged -= value;
        }
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded
        {
            add => _serverPark.ServerAdded += value;
            remove => _serverPark.ServerAdded -= value;
        }
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted
        {
            add => _serverPark.ServerDeleted += value;
            remove => _serverPark.ServerDeleted -= value;
        }

        public async Task<IMinecraftServer> CreateServer(string name, DataUser user)
        {
            var res = await _serverPark.CreateServer(name, user);
            return res;
        }

        public async Task DeleteServer(string name, DataUser user)
        {
            await _serverPark.DeleteServer(name, user);
        }

        public async Task RenameServer(string oldName, string newName, DataUser user)
        {
            await _serverPark.RenameServer(oldName, newName, user);
        }

        public async Task StartServer(string serverName, DataUser user)
        {
            await _serverPark.StartServer(serverName, user);
        }

        public async Task StopActiveServer(DataUser user)
        {
            await _serverPark.StopActiveServer(user);
        }

        public async Task ToggleServer(string serverName, DataUser user)
        {
            await _serverPark.ToggleServer(serverName, user);
        }
    }
}
