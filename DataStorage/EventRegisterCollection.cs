using DataStorage.Implementations.SQLite;
using DataStorage.Interfaces;

namespace DataStorage
{
    public class DatabaseAccess
    {
        public IDiscordDatabaseAccess DiscordEventRegister { get; internal init; } = null!;
        public IMinecraftDatabaseAccess MinecraftEventRegister { get; internal init; } = null!;
        public IWebsiteEventRegister WebsiteEventRegister { get; internal init; } = null!;
        public IServerParkEventRegister ServerParkEventRegister { get; internal init; } = null!;
        public IDatabaseSetup DatabaseSetup { get; init; } = null!;

        public async Task Setup(string connectionString) => 
            await DatabaseSetup.Setup(connectionString);


        public static DatabaseAccess SQLite { get; } = new DatabaseAccess()
        {
            DiscordEventRegister = new DiscordEventRegister(),
            MinecraftEventRegister = new MinecraftEventRegister(),
            WebsiteEventRegister = new WebsiteEventRegister(),
            ServerParkEventRegister = new ServerParkEventRegister(),
            DatabaseSetup = new DatabaseSetup()
        };
    }
}
