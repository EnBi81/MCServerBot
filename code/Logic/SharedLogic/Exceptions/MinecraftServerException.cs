using SharedPublic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Exceptions
{
    public class MinecraftServerException : MCExternalException
    {
        public MinecraftServerException(string? message = null) : base(message) { }
    }
}
