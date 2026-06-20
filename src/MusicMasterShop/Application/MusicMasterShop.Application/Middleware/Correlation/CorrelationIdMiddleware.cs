using Microsoft.AspNetCore.Http;

namespace MusicMasterShop.Application.Middleware.Correlation
{
    public class CorrelationIdMiddleware(RequestDelegate next)
    {
        private const string HeaderName = "X-Correlation-Id";

        public async Task InvokeAsync(HttpContext context, CorrelationId correlationId)
        {
            string? headerValue = context.Request.Headers[HeaderName].FirstOrDefault();
            correlationId.Set(headerValue);
            context.Response.Headers[HeaderName] = correlationId.Get();
            await next(context);
        }
    }
}
