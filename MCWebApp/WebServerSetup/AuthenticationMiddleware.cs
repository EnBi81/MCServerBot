using Application.PermissionControll;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.WebServerSetup
{
    public class AuthenticationMiddleware
    {
        private static string[] _allowedSites = { "login" };


        private readonly RequestDelegate _next;


        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            HttpRequest request = context.Request;

            //do your checkings
            if(!request.Path.StartsWithSegments("/login", StringComparison.OrdinalIgnoreCase))
            {
                if (request.Cookies[WebsitePermission.CookieName] is not string cookieValue || WebsitePermission.HasAccess(cookieValue))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }

            try
            {
                await _next(context);
            }
            catch
            {
                context.Response.StatusCode = 404;
            }
            
        }
    }
}
