using Application.DAOs;
using Application.DAOs.Database;
using DataStorageSQLite.Implementation.SQLite;
using DataStorageSQLite.Implementations.SQLite;

namespace DataStorageSQLite.Implementation
{
    public class DataStorageSQLiteImpl : IDatabaseAccess
    {
        public IMinecraftDataAccess MinecraftDataAccess { get; } = new MinecraftDAO();
        public IServerParkDataAccess ServerParkDataAccess { get; } = new ServerParkDAO();
        public IDatabaseSetup DatabaseSetup { get; } = new DatabaseSetup();
        public IPermissionDataAccess PermissionDataAccess { get; } = new PermissionDAO();
    }
}
