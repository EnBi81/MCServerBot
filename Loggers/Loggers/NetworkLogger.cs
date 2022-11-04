using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers.Loggers
{
    public class NetworkLogger : Logger
    {
        public void Log(string message) => AddLog("networking", message, ConsoleColor.White);
    }
}
