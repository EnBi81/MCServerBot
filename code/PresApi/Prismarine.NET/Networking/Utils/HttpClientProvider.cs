using System.Net.Http.Headers;

namespace Prismarine.NET.Networking.Utils;

/// <summary>
/// Provides a HttpClient instance.
/// </summary>
internal class HttpClientProvider
{
    private HttpClient? _client;


    public HttpClientProvider(string baseAddress) 
    {
        uri = new Uri(baseAddress); // for development purpose, later the user can set this
    }
    
    /// <summary>
    /// Gets the HttpClient instance.
    /// </summary>
    public HttpClient HttpClient {
        get
        {
            // if _client is null, create a new instance of HttpClient
            if(_client is null)
            {
                _client = new HttpClient { BaseAddress = uri };
            }
            
            if(JwtToken is not null)
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            else 
                _client.DefaultRequestHeaders.Authorization = null;
            
            
            return _client;
        }
    }

    /// <summary>
    /// Gets or sets the JWT token.
    /// </summary>
    public string? JwtToken { get; set; }

    private Uri? uri;
    /// <summary>
    /// Gets or sets the base address of the HttpClient instance.
    /// </summary>
    public string? BaseUrl
    {
        get => uri?.ToString();
    }
}
