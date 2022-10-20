using DataStorage.DataObjects.Enums;

namespace DataStorage.DataObjects.BaseInterfaces
{
    public interface IUserEvent : IBaseEvent
    {
        public ulong UserId { get; internal init; }
        public Platform Platform { get; internal init; }
    }
}
