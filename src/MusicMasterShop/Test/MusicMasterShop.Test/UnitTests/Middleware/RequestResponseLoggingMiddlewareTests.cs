using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicMasterShop.WebApi.Configuration.Logging;

namespace MusicMasterShop.Test.UnitTests.Middleware;

public sealed class RequestResponseLoggingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_LogsRequestAndResponseWithSensitiveValuesRedacted()
    {
        const string requestJson =
            """
            {
              "email": "seller@musicmaster.com",
              "senha": "secret123",
              "documentoCliente": "12345678901"
            }
            """;
        const string responseJson =
            """
            {
              "data": {
                "token": "jwt-secret-token",
                "id": "0dbd0486-e041-4ee8-9670-1762c2d6127d"
              }
            }
            """;

        var context = new DefaultHttpContext();
        byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(requestJson);
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/mmshop/v1/Auth/login";
        context.Request.ContentType = "application/json";
        context.Request.ContentLength = requestBytes.Length;
        context.Request.Body = new MemoryStream(requestBytes);
        context.Response.Body = new MemoryStream();

        bool downstreamReadRequest = false;
        RequestDelegate next = async httpContext =>
        {
            using var reader = new StreamReader(
                httpContext.Request.Body,
                leaveOpen: true);
            string body = await reader.ReadToEndAsync();
            downstreamReadRequest = body.Contains("seller@musicmaster.com");

            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(responseJson);
        };

        var logger = new TestLogger<WebApiTransactionLogMiddleware>();
        var options = Options.Create(new WebApiTransactionLogOptions());
        var middleware = new WebApiTransactionLogMiddleware(next, logger, options);
        var correlationId = new CorrelationId(
            new HttpContextAccessor { HttpContext = context });
        correlationId.Set("test-correlation-id");

        await middleware.InvokeAsync(context, correlationId);

        string logs = string.Join(Environment.NewLine, logger.Messages);

        Assert.True(downstreamReadRequest);
        Assert.Contains("seller@musicmaster.com", logs);
        Assert.Contains("[REDACTED]", logs);
        Assert.DoesNotContain("secret123", logs);
        Assert.DoesNotContain("12345678901", logs);
        Assert.DoesNotContain("jwt-secret-token", logs);
        Assert.Contains("0dbd0486-e041-4ee8-9670-1762c2d6127d", logs);
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull =>
            NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }

        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new();

            public void Dispose()
            {
            }
        }
    }
}
