using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.vtest.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ValuesController : ControllerBase
    {
        [MapToApiVersion("2.0")]
        [HttpGet("this")]
        public IActionResult Hi()
        {
            return Ok("Helllo v2");
        }
    }
}
