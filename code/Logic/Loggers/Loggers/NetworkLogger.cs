using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers.Loggers
{
    public class NetworkLogger : Logger
    {
#pragma warning disable CA1822 // Mark members as static
        public void Log(string message) => AddLog("networking", message, ConsoleColor.White);
#pragma warning restore CA1822 // Mark members as static
    }
}
