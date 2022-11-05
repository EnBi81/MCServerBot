using Loggers.Loggers;

namespace MCWebAPI.Middlewares
{
    public class MCApiLoggerMiddleware
    {
        private static ulong requestId = 0;

        private readonly RequestDelegate _next;
        private readonly WebApiLogger _logger;

        public MCApiLoggerMiddleware(RequestDelegate next, WebApiLogger logger)
        {
            _next = next;
            _logger = logger;
        }

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
