using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.Exceptions
{
    public class MCInternalException : MCException
    {
        public MCInternalException(string? message) : base(message) { }
    }
}
