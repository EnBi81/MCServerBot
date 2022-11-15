using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.Exceptions
{
    public class MCExternalException : MCException
    {
        public MCExternalException(string? message) : base(message) { }
    }
}
