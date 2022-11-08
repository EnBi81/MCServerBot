using MCWebAPI.WebSocketHandler;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace MCWebAPI.Controllers.Utils
{
    public class WebSocketActionResult : IActionResult
    {
        private readonly SocketPool _socketPool;
        private readonly ulong _userId;
        public WebSocketActionResult(SocketPool socketPool, ulong userId)
        {
            _socketPool = socketPool;
            _userId = userId;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {

            WebSocket ws = await context.HttpContext.WebSockets.AcceptWebSocketAsync();
            await _socketPool.AddSocket(_userId, ws);
        }
    }
}
