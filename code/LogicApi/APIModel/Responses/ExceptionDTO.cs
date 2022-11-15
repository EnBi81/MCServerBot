using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIModel.Responses
{
    public class ExceptionDTO
    {
        public string Message { get; set; } = string.Empty;
        public bool IsInternalException { get; set; } = false;
    }
}
