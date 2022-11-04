using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers
{
    public class HamachiLogger : Logger
    {
        public void Log(string message) => AddLog("hamachi", message, ConsoleColor.Cyan);
    }
}
