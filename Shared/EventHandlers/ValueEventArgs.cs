using Shared.Model;

namespace Shared.EventHandlers
{
    public class ValueEventArgs<T>
    {
        public T NewValue { get; set; }

        public ValueEventArgs(T newValue)
        {
            NewValue = newValue;
        }
    }
}
