using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class MCInternalException : MCException
    {
        public MCInternalException(string? message) : base(message) { }
    }
}
