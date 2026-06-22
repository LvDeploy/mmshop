using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using MusicMasterShop.Application.Middleware.Correlation;

namespace MusicMasterShop.WebApi.Configuration.Logging;

public sealed class WebApiTransactionLogMiddleware
{
    private const string TruncatedMarker = "...[TRUNCATED]";
    private readonly RequestDelegate _next;
    private readonly ILogger<WebApiTransactionLogMiddleware> _logger;
    private readonly WebApiTransactionLogOptions _options;
    private readonly HashSet<string> _sensitiveFields;

    public WebApiTransactionLogMiddleware(
        RequestDelegate next,
        ILogger<WebApiTransactionLogMiddleware> logger,
        IOptions<WebApiTransactionLogOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
        _sensitiveFields = new HashSet<string>(
            _options.SensitiveFields ?? [],
            StringComparer.OrdinalIgnoreCase);
    }

    public async Task InvokeAsync(HttpContext context, CorrelationId correlationId)
    {
        if (!ShouldLog(context.Request))
        {
            await _next(context);
            return;
        }

        string? requestPayload = await ReadRequestPayloadAsync(context.Request);
        ControllerActionDescriptor? action = context
            .GetEndpoint()?
            .Metadata
            .GetMetadata<ControllerActionDescriptor>();

        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId.Get(),
            ["ControllerName"] = action?.ControllerName,
            ["ActionName"] = action?.ActionName
        });

        _logger.LogInformation(
            "Controller request {HttpRequestMethod} {HttpRequestPath} payload {HttpRequestBody}",
            context.Request.Method,
            context.Request.Path.Value,
            requestPayload);

        Stream originalBody = context.Response.Body;
        await using var captureBody = new ResponseCaptureStream(
            originalBody,
            _options.MaxBodyLength + 1);
        context.Response.Body = captureBody;
        long startedAt = Stopwatch.GetTimestamp();

        try
        {
            await _next(context);

            string? responsePayload = ReadResponsePayload(
                context.Response.ContentType,
                captureBody.GetCapturedBytes());

            _logger.LogInformation(
                "Controller response {HttpRequestMethod} {HttpRequestPath} returned {HttpStatusCode} in {ElapsedMilliseconds} ms payload {HttpResponseBody}",
                context.Request.Method,
                context.Request.Path.Value,
                context.Response.StatusCode,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds,
                responsePayload);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Controller request {HttpRequestMethod} {HttpRequestPath} failed after {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path.Value,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);

            throw;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private bool ShouldLog(HttpRequest request)
    {
        return _options.Enabled
            && _options.MaxBodyLength > 0
            && request.Path.Value?.StartsWith(
                _options.PathPrefix,
                StringComparison.OrdinalIgnoreCase) == true;
    }

    private async Task<string?> ReadRequestPayloadAsync(HttpRequest request)
    {
        if (request.ContentLength == 0 || !IsSupportedContentType(request.ContentType))
            return null;

        request.EnableBuffering();
        request.Body.Position = 0;

        using var reader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 1024,
            leaveOpen: true);

        char[] buffer = new char[_options.MaxBodyLength + 1];
        int read = await reader.ReadBlockAsync(
            buffer.AsMemory(),
            request.HttpContext.RequestAborted);
        request.Body.Position = 0;

        bool truncated = read > _options.MaxBodyLength;
        string payload = new(
            buffer,
            0,
            Math.Min(read, _options.MaxBodyLength));

        return Sanitize(payload, request.ContentType, truncated);
    }

    private string? ReadResponsePayload(string? contentType, ReadOnlyMemory<byte> bytes)
    {
        if (bytes.IsEmpty || !IsSupportedContentType(contentType))
            return null;

        bool truncated = bytes.Length > _options.MaxBodyLength;
        string payload = Encoding.UTF8.GetString(
            bytes.Span[..Math.Min(bytes.Length, _options.MaxBodyLength)]);

        return Sanitize(payload, contentType, truncated);
    }

    private string Sanitize(string payload, string? contentType, bool truncated)
    {
        string sanitized = IsJson(contentType)
            ? RedactJson(payload)
            : payload;

        return truncated ? sanitized + TruncatedMarker : sanitized;
    }

    private string RedactJson(string payload)
    {
        try
        {
            JsonNode? node = JsonNode.Parse(payload);

            if (node is null)
                return payload;

            RedactNode(node);
            return node.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        catch (JsonException)
        {
            return "[INVALID JSON PAYLOAD]";
        }
    }

    private void RedactNode(JsonNode node)
    {
        if (node is JsonObject jsonObject)
        {
            foreach ((string propertyName, JsonNode? propertyValue) in jsonObject.ToList())
            {
                if (_sensitiveFields.Contains(propertyName))
                {
                    jsonObject[propertyName] = _options.RedactionValue;
                }
                else if (propertyValue is not null)
                {
                    RedactNode(propertyValue);
                }
            }

            return;
        }

        if (node is JsonArray jsonArray)
        {
            foreach (JsonNode? item in jsonArray)
            {
                if (item is not null)
                    RedactNode(item);
            }
        }
    }

    private static bool IsSupportedContentType(string? contentType)
    {
        return IsJson(contentType)
            || contentType?.StartsWith("text/", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool IsJson(string? contentType)
    {
        return contentType?.StartsWith(
                   "application/json",
                   StringComparison.OrdinalIgnoreCase) == true
            || contentType?.Contains(
                   "+json",
                   StringComparison.OrdinalIgnoreCase) == true;
    }

    private sealed class ResponseCaptureStream : Stream
    {
        private readonly Stream _inner;
        private readonly MemoryStream _capture;
        private readonly int _captureLimit;

        public ResponseCaptureStream(Stream inner, int captureLimit)
        {
            _inner = inner;
            _captureLimit = captureLimit;
            _capture = new MemoryStream(Math.Min(captureLimit, 4096));
        }

        public ReadOnlyMemory<byte> GetCapturedBytes() => _capture.ToArray();

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() => _inner.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken) =>
            _inner.FlushAsync(cancellationToken);

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
            Capture(buffer.AsSpan(offset, count));
        }

        public override async Task WriteAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken)
        {
            await _inner.WriteAsync(buffer.AsMemory(offset, count), cancellationToken);
            Capture(buffer.AsSpan(offset, count));
        }

        public override async ValueTask WriteAsync(
            ReadOnlyMemory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            await _inner.WriteAsync(buffer, cancellationToken);
            Capture(buffer.Span);
        }

        public override void WriteByte(byte value)
        {
            _inner.WriteByte(value);
            Capture([value]);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _capture.Dispose();

            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            await _capture.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        private void Capture(ReadOnlySpan<byte> bytes)
        {
            int remaining = _captureLimit - (int)_capture.Length;

            if (remaining <= 0)
                return;

            _capture.Write(bytes[..Math.Min(bytes.Length, remaining)]);
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) =>
            throw new NotSupportedException();

        public override void SetLength(long value) =>
            throw new NotSupportedException();
    }
}
