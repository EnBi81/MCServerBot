using Loggers.Loggers;

namespace MCWebAPI.Middlewares
{
    /// <summary>
    /// Logs all the requests and responses.
    /// </summary>
    public class ApiLoggerMiddleware
    {
        private static ulong requestId = 0;

        private readonly RequestDelegate _next;
        private readonly WebApiLogger _logger;

        /// <summary>
        /// Initializes the MCApiLoggerMiddleware.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ApiLoggerMiddleware(RequestDelegate next, WebApiLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Logs the request and response.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            ulong id = Interlocked.Increment(ref requestId);

            var request = context.Request;
            string logRequest = $"{id}-request: {request.Method} {request.Path}{request.QueryString.Value}";
            _logger.Log("middleware", logRequest);
            
            await _next(context);

            var response = context.Response;
            string logResponse = $"{id}-response: {response.StatusCode}";
            _logger.Log("middleware", logResponse);
        }
    }
}
