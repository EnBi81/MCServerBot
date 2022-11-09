using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.api
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public abstract class ApiController : MCControllerBase { }
}
