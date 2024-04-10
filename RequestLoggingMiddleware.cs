using ChatApplication2.DataAccessLayer.Models;
using System.Security.Claims;
using System.Text;
using ChatApplication2.DataAccessLayer.Data;

namespace ChatApplication2
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
            public async Task Invoke(HttpContext context, AppDbContext dbContext)
            {
                var request = context.Request;
                var requestBody = await GetRequestBody(request);
            

            var log = new ApiLog
            {
                IpAddress = GetIpAddress(context),
                RequestBody = requestBody,
                TimeStamp = DateTime.Now,
                Email = GetCurrentEmail(context),
                    //Email = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : string.Empty,
            //context.User.FindFirst(ClaimTypes.Email)
                };

                dbContext.ApiLogs.Add(log);
                await dbContext.SaveChangesAsync();

                await _next(context);
            }

            private async Task<string> GetRequestBody(HttpRequest request)
            {
                request.EnableBuffering();
                var body = string.Empty;

                using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    body = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }

                return body;
            }

            private string GetIpAddress(HttpContext context)
            {
                return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            }

        private string GetCurrentEmail(HttpContext context)
        {
            // Get the current user's claims principal
            var claimsPrincipal = context.User;

            // Get the email claim
            var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);

            return emailClaim?.Value;
        }

    }

        // Extension method used to add the middleware to the HTTP request pipeline.
        public static class RequestLoggingMiddlewareExtensions
        {
            public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder builder)
            {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
            }
        }
    

}
