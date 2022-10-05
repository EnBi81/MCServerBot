using Application.PermissionControll;
using Application.WebSocketHandler;
using Loggers;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace MCWebApp.Controllers.api.v1
{
    [ApiController]
    [Route("api/v1/ws/{accessCode}")]
    public class MCWebSocketController : ControllerBase
    {
        [HttpGet]
        public async void ForwardToWebsocket(string accessCode)
        {
            var context = Request.HttpContext;

            var ip = context.Connection.LocalIpAddress;

            if (!WebsitePermission.HasAccess(accessCode))
            {
                LogService.GetService<WebLogger>().Log("ws-request", $"WS request denied from ip {ip}: no access");
                return;
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                LogService.GetService<WebLogger>().Log("ws-request", "Websocket accepted for " + ip);

                WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();
                await SocketPool.SocketPoolInstance.AddSocket(accessCode, ws);
            }
            else
            {
                LogService.GetService<WebLogger>().Log("ws-request", "Not a websocket request: " + ip);
            }
        }
    }
}
