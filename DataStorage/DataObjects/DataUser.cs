using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.DataObjects
{
    public class DataUser
    {
        public ulong Id { get; internal init; }
        public string Username { get; internal init; } = null!;
        public string? ProfilePicUrl { get; internal init; }
    }
}
