using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers
{
    public class ServerParkController : ControllerBase
    {
        /*  
         *   get all servers (simplified):
         *       GET     api/v1/minecraftserver
         *   
         *   create server:
         *       POST    api/v1/minecraftserver
         *       body: { "new-name": "string" }
         *   
         *   
         *   get active server
         *       GET     api/v1/minecraftserver/active
         */
    }
}
