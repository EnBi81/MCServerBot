using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.DTOs
{
    public class AuthenticatedResponse
    {
        public string Jwt { get; set; } = "";
        public string Type { get; set; } = "";
    }
}
