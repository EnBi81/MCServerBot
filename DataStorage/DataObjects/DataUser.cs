using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.DataObjects
{
    public class DataUser
    {
        public static DataUser System { get; } = new DataUser()
        {
            Id = 1,
            Username = "System",
            ProfilePicUrl = null,
            WebAccessToken = null
        };

        public ulong Id { get; internal init; }
        public string Username { get; internal init; } = null!;
        public string? ProfilePicUrl { get; internal init; }
        public string? WebAccessToken { get; internal init; }
    }
}
