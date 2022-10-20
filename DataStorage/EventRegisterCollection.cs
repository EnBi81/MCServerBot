using DataStorage.Implementations.SQLite;
using DataStorage.Interfaces;
using System.Net.NetworkInformation;

namespace DataStorage
{
    public class EventRegisterCollection
    {
        public static IDiscordEventRegister DiscordEventRegister { get; } = new DiscordEventRegister();
        public static IMinecraftEventRegister MinecraftEventRegister { get; } = new MinecraftEventRegister();
    }
}
