using SharedPublic.Model;

namespace SharedPublic.EventHandlers
{
    public class ServerValueChangedEventArgs<T> : ServerValueEventArgs<T>
    {
        public T OldValue { get; init; }

        public ServerValueChangedEventArgs(T oldValue, T newValue, IMinecraftServer server) : base(newValue, server)
        {
            OldValue = oldValue;
        }
    }
}
