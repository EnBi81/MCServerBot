using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.Exceptions
{
    public class MCException : Exception
    {
        public MCException(string? message = null) : base(message) { }
        public MCException(Exception inner) : base(inner.Message, inner) { }
    }
}
