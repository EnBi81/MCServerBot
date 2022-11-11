using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.api.v1
{
    [ApiVersion("2.0")]
    [AllowAnonymous]
    public class TestController : ApiController
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Success!!!!!!");
        }
    }
}
