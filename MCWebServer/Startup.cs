using Application.Config;
using Loggers;
using Application.PermissionControll;
using Application.WebSocketHandler;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Web_Test
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            LogService.GetService<WebLogger>().Log("setup", "Setting up...");

            if (env.IsDevelopment())
            {
                LogService.GetService<WebLogger>().Log("setup", "Setting up development environment.");
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            LogService.GetService<WebLogger>().Log("setup", "Setting up websocket usage.");
            app.UseWebSockets(new WebSocketOptions() { KeepAliveInterval = TimeSpan.FromMinutes(2) });
            app.Use(ReceiveSockets);


            LogService.GetService<WebLogger>().Log("setup", "Setup completed");
            LogService.GetService<WebLogger>().Log("setup", "Listening on HTTPS port " + Config.Instance.WebServerPortHttps);
        }


        public async Task ReceiveSockets(HttpContext context, RequestDelegate next)
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
        }
    }
}
