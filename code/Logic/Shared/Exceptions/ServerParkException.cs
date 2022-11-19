using Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public class ServerParkException : MCExternalException
    {
        public ServerParkException(string? message = null) : base(message) { }
    }
}
