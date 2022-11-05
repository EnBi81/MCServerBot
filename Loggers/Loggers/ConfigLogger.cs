using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers.Loggers
{
    public class ConfigLogger : Logger
    {
#pragma warning disable CA1822 // Mark members as static
        public void Log(string message) => AddLog("config", message, ConsoleColor.Blue);

        public void LogError(Exception e) => AddLogException("config", e);
#pragma warning restore CA1822 // Mark members as static
    }
}
