using MCWebServer.PermissionControll;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;

namespace Web_Test.Pages
{
    public class IndexModel : PageModel
    {
        public string Code { get; set; }

        public void OnGet()
        {
            var cookieName = WebsitePermission.CookieName;

            if (Request.Query.ContainsKey("code"))
            {
                var code = Request.Query["code"];

                if (!WebsitePermission.HasAccess(code))
                {
                    RedirectToNoAccess();
                    return;
                }

                var cookieOption = new CookieOptions()
                {
                    Expires = DateTime.MaxValue
                };

                Response.Cookies.Append(cookieName, code, cookieOption);

                Response.Redirect("/");
            }

            else if (!Request.Cookies.ContainsKey(cookieName))
            {
                RedirectToNoAccess();
                return;
            }

            else
            {
                var code = Request.Cookies[cookieName];
                if (!WebsitePermission.HasAccess(code))
                {
                    RedirectToNoAccess();
                    return;
                }
            }
        }

        public void RedirectToNoAccess()
        {
            //Response.Redirect("/no-permission");
        }



        public static class BackgroundImageHelper
        {
            //bg-images-compressed/bg5-min.png
            public static string GetRandomImage()
            {
                Random r = new Random();

                DirectoryInfo info = new DirectoryInfo("wwwroot/bg-images-compressed");
                FileInfo[] files = info.GetFiles();

                FileInfo choosenImage = files[r.Next(files.Length)];

                return "bg-images-compressed/" + choosenImage.Name;
            }
        }
    }
}
