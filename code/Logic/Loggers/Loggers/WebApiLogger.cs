using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers.Loggers
{
    public class WebApiLogger : Logger
    {
#pragma warning disable CA1822 // Mark members as static
        public void Log(string source, string message) => AddLog("webapi-" + source, message, ConsoleColor.Yellow);

        public void LogError(string source, Exception e) => AddLogException("webapi-" + source, e);

        public void LogFatal(Exception e) => AddLogException("webapi-fatal", e, true);
#pragma warning restore CA1822 // Mark members as static
    }
}
