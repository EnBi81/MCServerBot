using APIModel.Responses;
using Loggers.Loggers;
using Shared.Exceptions;

namespace MCWebAPI.Middlewares
{
    internal class MCExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebApiLogger _logger;

        public MCExceptionHandlerMiddleware(RequestDelegate next, WebApiLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (MCExternalException e)
            {
                var errorMessage = new ExceptionDTO()
                {
                    Message = e.Message
                };

                string errorJson = Newtonsoft.Json.JsonConvert.SerializeObject(errorMessage);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400; 
                await context.Response.WriteAsync(errorJson);
                _logger.Log("-exception-client", e.Message);
            }
            catch (MCInternalException e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(e.Message);
                _logger.LogError("-exception-internal", e);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(e.Message);
                _logger.LogError("-exception-unexpected", e);
            }
        }
    }
}
