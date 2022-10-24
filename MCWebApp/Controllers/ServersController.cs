using Application.PermissionControll;
using DataStorage.Interfaces;
using MCWebApp.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers
{
    [Route("[controller]")]
    public class ServersController : Controller
    {
        private readonly IWebsiteEventRegister _websiteEventRegister;

        public ServersController(IWebsiteEventRegister websiteEventRegister)
        {
            _websiteEventRegister = websiteEventRegister;
        }


        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? token)
        {
            try
            {
                string? userToken = token ?? Request.Query[WebConstants.AUTH_COOKIE_NAME];

                if (await _websiteEventRegister.HasPermission(token ?? ""))
                    throw new Exception();

                var user = await _websiteEventRegister.GetUser(token!);

                ViewData["Token"] = userToken;
                ViewData["DiscordUser"] = user;
                ViewData["BGImage"] = GetRandomImage();

                return View();
            }
            catch 
            {
                if (token != null)
                {
                    return LocalRedirect($"~/login?redirectUrl=%2Fservers&token={token}");
                }
                
                return RedirectToPage("/noperm");
            }
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
