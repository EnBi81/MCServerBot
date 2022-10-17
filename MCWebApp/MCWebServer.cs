using Application.Minecraft;
using Application.PermissionControll;
using Application.WebSocketHandler;
using Loggers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using System.Net.WebSockets;

namespace MCWebApp
{
    internal static class MCWebServer
    {
        public static async Task StartWebServer(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton(IServerPark.Instance);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseMiddleware<AuthenticationMiddleware>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Index}/{action=Index}/{id?}");

            app.UseWebSockets(new WebSocketOptions() { KeepAliveInterval = TimeSpan.FromMinutes(2) });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapRazorPages();

            app.Use(ReceiveSockets);
            await app.StartAsync();
        }

        public static async Task ReceiveSockets(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path == "/ws")
            {
                string? ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
                LogService.GetService<WebLogger>().Log("ws-request", "Request received from " + ip);


                if (!context.Request.Query.ContainsKey(WebsitePermission.CookieName))
                {
                    LogService.GetService<WebLogger>().Log("ws-request", $"WS request denied from ip {ip}: no request query found"); return;
                }

                var code = context.Request.Query[WebsitePermission.CookieName];
                if (!WebsitePermission.HasAccess(code))
                {
                    LogService.GetService<WebLogger>().Log("ws-request", $"WS request denied from ip {ip}: no access");
                    return;
                }

                if (context.WebSockets.IsWebSocketRequest)
                {
                    LogService.GetService<WebLogger>().Log("ws-request", "Websocket accepted for " + ip);

                    WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();
                    await SocketPool.SocketPoolInstance.AddSocket(code, ws);
                }
                else
                {
                    LogService.GetService<WebLogger>().Log("ws-request", "Not a websocket request: " + ip);
                }
            }
            else
                await next(context);
        }
    }
}
