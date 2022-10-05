using Application.PermissionControll;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers
{
    [Controller]
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
            string? cookieValue = Request.Cookies[WebsitePermission.CookieName];
            DiscordUser? user = WebsitePermission.GetUser(cookieValue ?? "");

            Console.WriteLine("Checking user");

            if (user == null)
            {
                Console.WriteLine("User is null");
                if (Request.Query.ContainsKey("token"))
                {
                    Console.WriteLine("Redirecting to login");
                    return RedirectToAction("/login?redirectUrl=%2Findex&" + Request.QueryString);
                }
                else
                {
                    Console.WriteLine("Redirecting to noperm");
                    return RedirectToAction("/noperm");
                }
            }
            
            ViewData["Token"] = cookieValue;
            ViewData["DiscordUser"] = user;

            return View();
        }
    }
}
