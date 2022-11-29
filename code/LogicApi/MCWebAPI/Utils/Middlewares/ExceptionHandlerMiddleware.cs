using APIModel.Responses;
using Loggers.Loggers;
using SharedPublic.Exceptions;

namespace MCWebAPI.Utils.Middlewares
{
    internal class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebApiLogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, WebApiLogger logger)
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
                WriteContext(context, 400, e.Message, false);
                _logger.Log("-exception-client", e.Message);
            }
            catch (MCInternalException e)
            {
                WriteContext(context, 500, e.Message, true);
                _logger.LogError("-exception-internal", e);
            }
            catch (Exception e)
            {
                WriteContext(context, 500, e.Message, true);
                _logger.LogError("-exception-unexpected", e);
            }
        }

        private static void WriteContext(HttpContext context, int statuscode, string message, bool isInternal)
        {
            string jsonMessage = GetJsonException(message, isInternal);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statuscode;
            context.Response.WriteAsync(jsonMessage);
        }

        private static string GetJsonException(string message, bool isInternalException)
        {
            var errorMessage = new ExceptionDTO()
            {
                Message = message,
                IsInternalException = isInternalException
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(errorMessage);
        }
    }
}
