using DataStorage.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Interfaces
{
    public interface IWebsiteEventRegister
    {

        Task<bool> HasPermission(string token);
    }
}
