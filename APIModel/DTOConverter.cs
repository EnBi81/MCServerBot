using APIModel.DTOs;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIModel
{
    public static class DTOConverter
    {
        public static MinecraftServerDTO ToDTO(this IMinecraftServer server) => new MinecraftServerDTO(server);
    }
}
