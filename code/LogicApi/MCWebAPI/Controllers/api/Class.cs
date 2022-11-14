using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.api
{
    [ApiVersion("2.0")]
    public class Class : ApiController
    {
        [HttpGet]
        public string Get()
        {
            return "value";
        }
    }
}
