using Newtonsoft.Json;
using System.Net;

namespace key_vault.Services
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                var code = HttpStatusCode.InternalServerError;

                var content = new
                {
                    Exception = ex.GetType(),
                    Error = ex.Message,
                    Stack = ex.StackTrace
                };

                var result = JsonConvert.SerializeObject(content);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)code;
                await httpContext.Response.WriteAsync(result);
            }
        }
    }
}
