﻿using Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIModel.APIExceptions
{
    public class WebApiArgumentException : MCExternalException
    {
        public WebApiArgumentException(string? message = null) : base(message) { }
    }
}
