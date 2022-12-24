using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.DTOs;

public class CommandResponse
{
    public required string Response { get; init; }
    public required bool IsValidResponse { get; init; }
}
