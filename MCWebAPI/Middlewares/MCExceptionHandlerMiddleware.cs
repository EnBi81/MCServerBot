using APIModel.Responses;
using Shared.Exceptions;

namespace MCWebAPI.Middlewares
{
    internal class MCExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public MCExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (MCException e)
            {
                var errorMessage = new ExceptionDTO()
                {
                    Message = e.Message
                };

                string errorJson = Newtonsoft.Json.JsonConvert.SerializeObject(errorMessage);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400; 
                await context.Response.WriteAsync(errorJson);
            }
        }
    }
}
