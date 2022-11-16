using Application.Minecraft;
using MCWebAPI.SignalR.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MCWebAPI.Controllers.api
{
    [ApiVersion("2.0")]
    [AllowAnonymous]
    public class ValuesController : ApiController
    {
        private readonly IHubContext<ServerParkHub> _hubContext;

        public ValuesController(IHubContext<ServerParkHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Get(string text)
        {
            _hubContext.Clients.All.SendAsync("Receive", text);
            return Ok();
        }
    }
}
