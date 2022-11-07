using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIModel.Responses
{
    public class AuthenticatedResponse
    {
        public string Token { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
}
