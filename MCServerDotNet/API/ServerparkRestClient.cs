using Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServerDotNet.API
{
    public class ServerparkRestClient
    {
        public Platform Platform { get; init; } = Platform.Website;

        public ServerparkRestClient(string url)
        {

        }

        public async Task Login(string token)
        {

        }
    }
}
