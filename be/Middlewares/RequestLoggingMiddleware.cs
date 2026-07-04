namespace Backend.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {context.Request.Method} {context.Request.Path}");
            await _next(context);
        }
    }

}