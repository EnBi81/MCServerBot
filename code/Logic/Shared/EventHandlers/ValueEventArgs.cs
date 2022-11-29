using SharedPublic.Model;

namespace SharedPublic.EventHandlers
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
