using System.Net.Http.Headers;

namespace Prismarine.NET.Networking.Utils;

/// <summary>
/// Provides a HttpClient instance.
/// </summary>
internal class HttpClientProvider
{
    public static HttpClientProvider Instance = new HttpClientProvider();


    private HttpClient? _client;


    private HttpClientProvider() 
    {
        BaseUrl = "https://localhost:7229"; // for development purpose, later the user can set this
    }
    
    /// <summary>
    /// Gets the HttpClient instance.
    /// </summary>
    public HttpClient HttpClient {
        get
        {
            // if _client is null, create a new instance of HttpClient
            _client ??= new HttpClient();

            if(JwtToken is not null)
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            else 
                _client.DefaultRequestHeaders.Authorization = null;

            if (uri is not null)
                _client.BaseAddress = uri;

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
        set => uri = value is not null ? new Uri(value) : null;
    }
}
