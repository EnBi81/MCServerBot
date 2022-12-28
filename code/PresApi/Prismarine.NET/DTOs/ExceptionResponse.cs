using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.DTOs
{
    internal class ExceptionResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool IsInternalException { get; set; }
    }
}
