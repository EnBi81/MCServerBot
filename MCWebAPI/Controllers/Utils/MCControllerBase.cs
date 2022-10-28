using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MCWebAPI.Controllers.Utils
{
    public class MCControllerBase : ControllerBase
    {
        public ulong GetUserId => ulong.Parse(User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Sid))!.Value);


        public IActionResult GetBadRequest(string message)
        {
            var errorMessage = FormatErrorMessage(message);
            return BadRequest(errorMessage);
        }


        public static object FormatErrorMessage(string errorMessage)
        {
            return new { ErrorMessage = errorMessage };
        }
    }
}
