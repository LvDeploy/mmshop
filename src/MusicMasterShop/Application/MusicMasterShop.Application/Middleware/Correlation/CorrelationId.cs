using Microsoft.AspNetCore.Http;

namespace MusicMasterShop.Application.Middleware.Correlation
{
    public class CorrelationId(IHttpContextAccessor httpContextAccessor)
    {
        private const string HeaderName = "X-Correlation-Id";
        private static readonly AsyncLocal<string?> Current = new();

        public string Get()
        {
            return httpContextAccessor.HttpContext?.Items[HeaderName]?.ToString()
                ?? Current.Value
                ?? string.Empty;
        }

        public void Set(string? value)
        {
            Current.Value = string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString() : value;

            if (httpContextAccessor.HttpContext is not null)
            {
                httpContextAccessor.HttpContext.Items[HeaderName] = Current.Value;
            }
        }
    }
}
