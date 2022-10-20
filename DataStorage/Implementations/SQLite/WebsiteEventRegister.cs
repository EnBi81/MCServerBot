using DataStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Implementations.SQLite
{
    internal class WebsiteEventRegister : IWebsiteEventRegister
    {
        public Task<bool> HasPermission(string token)
        {
            throw new NotImplementedException();
        }
    }
}
