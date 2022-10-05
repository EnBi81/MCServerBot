using Application.PermissionControll;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers
{
    [Route("[controller]")]
    public class ServersController : Controller
    {
        [HttpGet]
        public IActionResult Index([FromQuery] string? token)
        {
            string? cookieValue = Request.Cookies[WebsitePermission.CookieName];
            DiscordUser? user = WebsitePermission.GetUser(cookieValue ?? "");

            if (user == null)
            {
                Console.WriteLine("User is null");
                if (token != null)
                {
                    return LocalRedirect($"~/login?redirectUrl=%2Fservers&token={token}");
                }
                else
                {
                    return RedirectToPage("/noperm");
                }
            }

            ViewData["Token"] = cookieValue;
            ViewData["DiscordUser"] = user;
            ViewData["BGImage"] = GetRandomImage();


            return View();
        }

        //bg-images-compressed/bg5-min.png
        private static string GetRandomImage()
        {
            Random r = new Random();

            DirectoryInfo info = new DirectoryInfo("wwwroot/index/bg-images-compressed");
            FileInfo[] files = info.GetFiles();

            FileInfo choosenImage = files[r.Next(files.Length)];

            return "index/bg-images-compressed/" + choosenImage.Name;
        }
    }
}
