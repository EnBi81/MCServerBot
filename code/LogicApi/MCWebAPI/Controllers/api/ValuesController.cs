using MCWebAPI.SignalR.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MCWebAPI.Controllers.api
{
    [ApiVersion("2.0")]
    public class ValuesController : ApiController
    {
        private readonly IHubContext<ServerParkHub> _hubContext;

        public ValuesController(IHubContext<ServerParkHub> context)
        {
            _hubContext = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public void Post(string text)
        {
            _hubContext.Clients.All.SendAsync("Receive", text);
            
        }
    }
}
