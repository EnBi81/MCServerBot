using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.Exceptions
{
    /// <summary>
    /// Is thrown when the user is not authenticated but tries to access an authenticated endpoint
    /// </summary>
    public class AuthenticationException : ApiException
    {
        public AuthenticationException() : base("You are not logged!", false)
        {
        }
    }
}
