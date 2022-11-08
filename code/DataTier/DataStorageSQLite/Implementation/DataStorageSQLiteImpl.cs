using Application.DAOs;
using Application.DAOs.Database;
using DataStorageSQLite.Implementation.SQLite;
using DataStorageSQLite.Implementations.SQLite;

namespace DataStorageSQLite.Implementation
{
    /// <summary>
    /// Holding the data access objects.
    /// </summary>
    public class DataStorageSQLiteImpl : IDatabaseAccess
    {
        /// <inheritdoc/>
        public IMinecraftDataAccess MinecraftDataAccess { get; } = new MinecraftDAO();
        /// <inheritdoc/>
        public IServerParkDataAccess ServerParkDataAccess { get; } = new ServerParkDAO();
        /// <inheritdoc/>
        public IDatabaseSetup DatabaseSetup { get; } = new DatabaseSetup();
        /// <inheritdoc/>
        public IPermissionDataAccess PermissionDataAccess { get; } = new PermissionDAO();
    }
}
