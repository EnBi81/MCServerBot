using APIModel.APIExceptions;
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
    [Authorize]
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ForwardToWebsocket()
        {
            var context = Request.HttpContext;
            var ip = context.Connection.LocalIpAddress;

            var user = await GetUserEventData();

            if (context.WebSockets.IsWebSocketRequest)
                return new WebSocketActionResult(_socketPool, user.Id);

            throw new WebsocketException("Request is not a websocket");
        }
    }
}
