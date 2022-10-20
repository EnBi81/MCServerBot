using DataStorage.DataObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.DataObjects
{
    public class UserEventData
    {
        public ulong Id { get; init; }
        public string Username { get; init; }
        public Platform Platform { get; init; }
    }
}
