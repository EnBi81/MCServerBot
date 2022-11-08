using MCWebAPI.APIExceptions;
using MCWebAPI.Controllers.Utils;
using MCWebAPI.WebSocketHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers
{
    /// <summary>
    /// Handles the websocket request
    /// </summary>
    [ApiController]
    [Route("ws")]
    [Authorize]
    public class MCWebSocketController : MCControllerBase
    {
        private readonly SocketPool _socketPool;

        /// <summary>
        /// Initializes the MCWebSocketController
        /// </summary>
        /// <param name="socketPool"></param>
        public MCWebSocketController(SocketPool socketPool)
        {
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
