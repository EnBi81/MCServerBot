using Prismarine.NET.Networking.Implementations;
using Prismarine.NET.Networking.Interfaces;
using System.Net;
using System.Text.RegularExpressions;

namespace Sandbox
{
    public class SandBoxClass
    {
        static async Task Main(string[] args)
        {
            IAuthService authService = new AuthHttpClient();

            var response = await authService.Login("test-acc");
            Console.WriteLine("Token: " + response.Jwt);
        }
    }
}

