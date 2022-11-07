﻿using Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebAPI.APIExceptions
{
    public class ConfigException : MCInternalException
    {
        public ConfigException(string? message = null) : base(message) { }
         
    }
}