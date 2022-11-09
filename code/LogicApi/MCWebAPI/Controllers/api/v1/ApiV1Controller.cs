using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.api.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public abstract class ApiV1Controller : ApiController { }
}
