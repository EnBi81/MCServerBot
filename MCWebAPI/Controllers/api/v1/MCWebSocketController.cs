using Application.DAOs.Database;
using Loggers;
using MCWebAPI.WebSocketHandler;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace MCWebAPI.Controllers.api.v1
{
    [ApiController]
    [Route("api/v1/ws/{accessCode}")]
    public class MCWebSocketController : ControllerBase
    {
        private readonly IPermissionDataAccess _permissionDataAccess;
        private readonly SocketPool _socketPool;

        public MCWebSocketController(IPermissionDataAccess permissionDataAccess, SocketPool socketPool)
        {
            _permissionDataAccess = permissionDataAccess;
            _socketPool = socketPool;
        }

        [HttpGet]
        public async Task ForwardToWebsocket(string accessCode)
        {
            var context = Request.HttpContext;

            var ip = context.Connection.LocalIpAddress;

            if (!await _permissionDataAccess.HasPermission(accessCode))
            {
                LogService.GetService<WebLogger>().Log("ws-request", $"WS request denied from ip {ip}: no access");
                return;
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                LogService.GetService<WebLogger>().Log("ws-request", "Websocket accepted for " + ip);

                WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();
                await _socketPool.AddSocket(accessCode, ws);
            }
            else
            {
                LogService.GetService<WebLogger>().Log("ws-request", "Not a websocket request: " + ip);
            }
        }
    }
}
