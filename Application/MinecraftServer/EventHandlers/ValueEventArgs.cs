namespace Application.MinecraftServer.EventHandlers
{
    public class ValueEventArgs<T> : EventArgs
    {
        public T NewValue { get; set; }

        public ValueEventArgs(T newValue)
        {
            NewValue = newValue;
        }
    }
}
