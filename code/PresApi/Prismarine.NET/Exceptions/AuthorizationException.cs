using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.Exceptions
{
    /// <summary>
    /// Is thrown when the user is not authorized to access an endpoint
    /// </summary>
    public class AuthorizationException : ApiException
    {
        public AuthorizationException() : base("You are not allowed to use this function", false)
        {
        }
    }
}
