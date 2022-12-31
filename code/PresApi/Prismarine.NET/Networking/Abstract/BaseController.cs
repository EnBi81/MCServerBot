using Prismarine.NET.DTOs;
using Prismarine.NET.Exceptions;
using Prismarine.NET.Networking.Utils;
using System.Net;
using System.Net.Http.Json;

namespace Prismarine.NET.Networking.Abstract
{
    internal abstract class BaseController
    {
        private HttpClientProvider HttpClientProvider { get; } 
        private JsonSerializer Serializer { get; }


        public BaseController(HttpClientProvider httpClientProvider, JsonSerializer jsonSerializer)
        {
            HttpClientProvider = httpClientProvider;
            Serializer = jsonSerializer;
        }


        protected async Task<T> GetAsync<T>(string uri)
        {
            // get the http client
            HttpClient client = HttpClientProvider.HttpClient;

            HttpResponseMessage response = await client.GetAsync(uri);
            await ThrowExceptionIfRequestUnsuccessful(response);

            var content = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(content);
            return Serializer.Deserialize<T>(content) ?? throw new Exception("Deserialization failed");
        }

        #region Post
        /// <summary>
        /// Creates a post request and deserializes the response body
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected async Task<T> PostAsync<T>(string uri, object body)
        {
            var response = await PostInternalAsync(uri, body);
            var content = await response.Content.ReadAsStringAsync();
            var deserialized = Serializer.Deserialize<T>(content);

            if(deserialized is null)
                throw new Exception("Deserialization failed");

            return deserialized;
        }
        
        /// <summary>
        /// Creates a post request without expecting a response body
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        protected async Task PostAsync(string uri, object body) =>
            await PostInternalAsync(uri, body);
        
        private async Task<HttpResponseMessage> PostInternalAsync(string uri, object body)
        {
            // get the http client
            HttpClient client = HttpClientProvider.HttpClient;
            // serialize the content to json
            string json = Serializer.Serialize(body);

            // make a post request to the server
            HttpResponseMessage result = await client.PostAsJsonAsync(uri, body);
            await ThrowExceptionIfRequestUnsuccessful(result);
            return result;
        }
        #endregion


        #region Util methods
        
        /// <summary>
        /// Throws exceptions if the response is not successful
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="AuthenticationException">if the user is not logged in</exception>
        /// <exception cref="AuthorizationException">if the user has no permission</exception>
        /// <exception cref="ApiException">other exceptions</exception>
        /// <exception cref="HttpRequestException">unknown exceptions</exception>
        private async Task ThrowExceptionIfRequestUnsuccessful(HttpResponseMessage response)
        {
            // check if the response is successful
            if (!response.IsSuccessStatusCode)
            {
                // the user is not logged in
                if (response.StatusCode is HttpStatusCode.Unauthorized)
                    throw new AuthenticationException();

                // the user has no permission to access the resource
                if (response.StatusCode is HttpStatusCode.Unauthorized)
                    throw new AuthorizationException();

                // server returned error
                string errorContent = await response.Content.ReadAsStringAsync();
                ExceptionResponse? error = Serializer.Deserialize<ExceptionResponse>(errorContent);

                if (error is not null)
                    throw new ApiException(error.Message, error.IsInternalException);

                // if everything else fails, throw an unhandled exception
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
        }

        #endregion
    }
}
