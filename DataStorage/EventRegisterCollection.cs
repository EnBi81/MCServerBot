using DataStorage.Implementations.SQLite;
using DataStorage.Interfaces;

namespace DataStorage
{
    public class EventRegisterCollection
    {
        public static IDiscordEventRegister DiscordEventRegister { get; } = new DiscordEventRegister();
        public static IMinecraftEventRegister MinecraftEventRegister { get; } = new MinecraftEventRegister();
        public static IWebsiteEventRegister WebsiteEventRegister { get; } = new WebsiteEventRegister();
        public static IServerParkEventRegister ServerParkEventRegister { get; }
    }
}
