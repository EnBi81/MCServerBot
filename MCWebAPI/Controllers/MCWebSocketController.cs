using Application.DAOs.Database;
using Loggers;
using MCWebAPI.Controllers.Utils;
using MCWebAPI.WebSocketHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers
{
    [ApiController]
    [Route("ws")]
    public class MCWebSocketController : MCControllerBase
    {
        private readonly IPermissionDataAccess _permissionDataAccess;
        private readonly SocketPool _socketPool;

        public MCWebSocketController(IPermissionDataAccess permissionDataAccess, SocketPool socketPool)
        {
            _permissionDataAccess = permissionDataAccess;
            _socketPool = socketPool;
        }

        [HttpGet]
        public async Task<IActionResult> ForwardToWebsocket(string accessCode)
        {
            var context = Request.HttpContext;

            var ip = context.Connection.LocalIpAddress;

            if (!await _permissionDataAccess.HasPermission(accessCode))
            {
                LogService.GetService<WebLogger>().Log("ws-request", $"WS request denied from ip {ip}: no access");
                return BadRequest("User denied");
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                LogService.GetService<WebLogger>().Log("ws-request", "Websocket accepted for " + ip);

                return new WebSocketActionResult(_socketPool, 0);
            }

            return GetBadRequest("Not a websocket request.");
        }
    }
}
