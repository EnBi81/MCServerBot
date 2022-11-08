using Application.PermissionControll;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers
{
    [Controller]
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Hello");
        }
    }
}
