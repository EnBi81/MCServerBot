namespace DataStorage.DataObjects.BaseInterfaces
{
    public interface IServerEvent : IBaseEvent
    {
        public ulong ServerId { get; internal init; }
    }
}
