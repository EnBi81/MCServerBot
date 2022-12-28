using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.Exceptions
{
    /// <summary>
    /// Prismarine Api Exception
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Gets if the exception is a server side exception (internal = true) or a client side exception (internal = false)
        /// </summary>
        public bool IsInternal { get; }
        
        public ApiException(string message, bool isInternal) : base(message)
        {
            IsInternal = isInternal;
        }
    }
}
