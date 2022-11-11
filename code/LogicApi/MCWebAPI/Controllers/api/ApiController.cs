using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.api
{
    /// <summary>
    /// Base Api Controller configuring the input and output types and the basic route to an api controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Authorize]
    public abstract class ApiController : MCControllerBase
    {
        /// <summary>
        /// Api version 1.0
        /// </summary>
        public const string ApiVersionV1 = "1.0";
    }
}
