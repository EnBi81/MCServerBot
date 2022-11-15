using SharedPublic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebAPI.APIExceptions
{
    public class WebsocketException : MCExternalException
    {
        public WebsocketException(string? message = null) : base(message) { }
    }
}
