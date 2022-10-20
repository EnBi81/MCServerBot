namespace DataStorage.DataObjects.BaseInterfaces
{
    public interface IBaseEvent
    {
        public ulong EventId { get; internal init; }
        public DateTime DateTime { get; internal init; }
    }
}
