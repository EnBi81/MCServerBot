using Application.DAOs;
using Application.DAOs.Database;
using Shared.DTOs;
using Shared.EventHandlers;
using Shared.Model;

namespace Application.Minecraft
{
    /// <summary>
    /// Proxy object for ServerPark, it handles all the database registrations.
    /// </summary>
    public class ServerPark : IServerPark
    {
        private readonly IServerParkDataAccess _serverParkEventRegister;
        private readonly ServerParkLogic _serverPark;

        private bool _initialized = false;

        public ServerPark(IDatabaseAccess databaseAccess, MinecraftConfig config)
        {
            _serverParkEventRegister = databaseAccess.ServerParkDataAccess;
            _serverPark = new ServerParkLogic(databaseAccess, config);
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            await _serverPark.InitializeAsync();
            _initialized = true;
        }

        /// <summary>
        /// Throws an exception if the instance is not initialized.
        /// </summary>
        /// <exception cref="Exception">when the instance is not initialized.</exception>
        private void ThrowExceptionIfNotInitialized()
        {
            if (!_initialized)
                throw new Exception("ServerPark not initialized!");
        }

        /// <inheritdoc/>
        public IMinecraftServer? ActiveServer 
        { 
            get
            { 
                ThrowExceptionIfNotInitialized(); 
                return _serverPark.ActiveServer;
            } 
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<long, IMinecraftServer> MCServers
        { 
            get 
            { 
                ThrowExceptionIfNotInitialized();
                return _serverPark.MCServers; 
            } 
        } 

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange
        {
            add 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerChange += value; 
            }
            remove
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerChange -= value;
            }
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<ServerStatus>> ActiveServerStatusChange
        {
            add 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerStatusChange += value; 
            }
            remove
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerStatusChange -= value; 
            }
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<ILogMessage>> ActiveServerLogReceived
        {
            add 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerLogReceived += value;
            }
            remove
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerLogReceived -= value;
            }
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerJoined
        {
            add 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerPlayerJoined += value;
            }
            remove 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerPlayerJoined -= value;
            }
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerLeft
        {
            add 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerPlayerLeft += value;
            }
            remove 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerPlayerLeft -= value;
            }
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<(string CPU, string Memory)>> ActiveServerPerformanceMeasured
        {
            add
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerPerformanceMeasured += value;
            }
            remove 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ActiveServerPerformanceMeasured -= value;
            }
        }
        /// <inheritdoc/>
        public event EventHandler<ValueChangedEventArgs<string>> ServerNameChanged
        {
            add 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ServerNameChanged += value;
            }
            remove
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ServerNameChanged -= value;
            }
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded
        {
            add
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ServerAdded += value;
            }
            remove
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ServerAdded -= value;
            }
        }
        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted
        {
            add 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ServerDeleted += value;
            }
            remove 
            {
                ThrowExceptionIfNotInitialized();
                _serverPark.ServerDeleted -= value;
            }
        }



        /// <inheritdoc/>
        public async Task<IMinecraftServer> CreateServer(string serverName, UserEventData user)
        {
            ThrowExceptionIfNotInitialized();

            var res = await _serverPark.CreateServer(serverName, user);
            await _serverParkEventRegister.CreateServer(res.Id, res.ServerName, user);

            return res;
        }

        /// <inheritdoc/>
        public async Task<IMinecraftServer> DeleteServer(long id, UserEventData user)
        {
            ThrowExceptionIfNotInitialized();

            var server = await _serverPark.DeleteServer(id, user);
            await _serverParkEventRegister.DeleteServer(server.Id, user);

            return server;
        }

        /// <inheritdoc/>
        public async Task<IMinecraftServer> RenameServer(long id, string newName, UserEventData user)
        {
            ThrowExceptionIfNotInitialized();

            var server = await _serverPark.RenameServer(id, newName, user);
            await _serverParkEventRegister.RenameServer(server.Id, newName, user);

            return server;
        }

        /// <inheritdoc/>
        public async Task StartServer(long id, UserEventData user)
        {
            ThrowExceptionIfNotInitialized();

            await _serverPark.StartServer(id, user);

            var server = ActiveServer;
            await _serverParkEventRegister.StartServer(server!.Id, user);
        }

        /// <inheritdoc/>
        public async Task StopActiveServer(UserEventData user)
        {
            ThrowExceptionIfNotInitialized();

            await _serverPark.StopActiveServer(user);

            var server = ActiveServer;
            await _serverParkEventRegister.StopServer(server!.Id, user);
        }

        /// <inheritdoc/>
        public async Task ToggleServer(long id, UserEventData user)
        {
            ThrowExceptionIfNotInitialized();

            bool isRunning = ActiveServer?.IsRunning ?? false;

            await _serverPark.ToggleServer(id, user);

            var server = ActiveServer;
            if (isRunning)
                await _serverParkEventRegister.StopServer(server!.Id, user);
            else
                await _serverParkEventRegister.StartServer(server!.Id, user);
        }

        public IMinecraftServer GetServer(long id)
        {
            ThrowExceptionIfNotInitialized();
            return _serverPark.GetServer(id);
        }
    }
}
