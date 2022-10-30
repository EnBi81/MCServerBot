using Application.DAOs.Database;

namespace Application.DAOs
{ 
    /// <summary>
    /// Holding the data access objects.
    /// </summary>
    public interface IDatabaseAccess
    {
        /// <summary>
        /// Permission Data Access Object.
        /// </summary>
        public IPermissionDataAccess PermissionDataAccess { get; }
        /// <summary>
        /// Minecraft Data Access Object.
        /// </summary>
        public IMinecraftDataAccess MinecraftDataAccess { get; } 
        /// <summary>
        /// ServerPark Data Access Object.
        /// </summary>
        public IServerParkDataAccess ServerParkDataAccess { get; }
        /// <summary>
        /// Database setup object.
        /// </summary>
        public IDatabaseSetup DatabaseSetup { get; } 
    }
}
