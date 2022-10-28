using Application.DAOs.Database;

namespace Application.DAOs
{
    public interface IDatabaseAccess
    {
        public IPermissionDataAccess PermissionDataAccess { get; }
        public IMinecraftDataAccess MinecraftDataAccess { get; } 
        public IServerParkDataAccess ServerParkDataAccess { get; }
        public IDatabaseSetup DatabaseSetup { get; } 
    }
}
