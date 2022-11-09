using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.vtest.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ValuesController : ControllerBase
    {

        [MapToApiVersion("1.0")]
        [HttpGet("this")]
        public IActionResult Hiv1()
        {
            return Ok("Helllo v1");
        }
    }
}
