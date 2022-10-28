using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServerDotNet.API.Internal.NetHelpers
{
    internal class HttpCreator
    {

        public static async Task<string> Get(string uri)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
            string text = await response.Content.ReadAsStringAsync();

            return text;
        }
    }

    internal static class HttpExtensions
    {
        public static HttpClient WithAuthenticationToken(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return httpClient;
        }
    }
}
