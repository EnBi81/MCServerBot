using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers.Loggers
{
    public class WebApiLogger : Logger
    {
        public void Log(string source, string message) => AddLog("webapi-" + source, message, ConsoleColor.Yellow);

        public void LogError(string source, Exception e) => AddLogException("webapi-" + source, e);
    }
}
