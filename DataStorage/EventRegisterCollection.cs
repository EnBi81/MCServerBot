using DataStorage.Implementations.SQLite;
using DataStorage.Interfaces;

namespace DataStorage
{
    public class DatabaseAccess
    {
        public IDiscordEventRegister DiscordEventRegister { get; internal init; } = null!;
        public IMinecraftEventRegister MinecraftEventRegister { get; internal init; } = null!;
        public IWebsiteEventRegister WebsiteEventRegister { get; internal init; } = null!;
        public IServerParkEventRegister ServerParkEventRegister { get; internal init; } = null!;
        public IDatabaseSetup DatabaseSetup { get; internal init; } = null!;


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
