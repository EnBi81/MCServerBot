using Shared.Model;

namespace Shared.EventHandlers
{
    public class ServerValueEventArgs<T> : ValueEventArgs<T>
    {
        public IMinecraftServer Server { get; set; }

        public ServerValueEventArgs(T newValue, IMinecraftServer server) : base(newValue)
        {
            Server = server;
        }
    }
}
