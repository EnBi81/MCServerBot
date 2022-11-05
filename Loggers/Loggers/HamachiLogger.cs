using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers
{
    public class HamachiLogger : Logger
    {
#pragma warning disable CA1822 // Mark members as static
        public void Log(string message) => AddLog("hamachi", message, ConsoleColor.Cyan);
#pragma warning restore CA1822 // Mark members as static
    }
}
