﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions 
{ 
    
    public class MCPermissionException : MCExternalException
    {
        public MCPermissionException(string? message = null) : base(message) { }
    }
}
