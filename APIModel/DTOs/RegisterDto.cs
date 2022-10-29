using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIModel.DTOs
{
    public class RegisterDto
    {
        public ulong Id { get; set; }
        public string? DiscordName { get; set; }
        public string? ProfilePic { get; set; }
    }
}
