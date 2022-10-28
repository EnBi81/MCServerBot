namespace Shared.EventHandlers
{
    public class ValueChangedEventArgs<T> : ValueEventArgs<T>
    {
        public T OldValue { get; init; }

        public ValueChangedEventArgs(T oldValue, T newValue) : base(newValue)
        {
            OldValue = oldValue;
        }
    }
}
