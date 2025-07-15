using System.Net;
using System.Net.Mime;
using System.Text;
using BtmsGatewayStub.Utils;
using Microsoft.Extensions.Primitives;
using ILogger = Serilog.ILogger;

namespace BtmsGatewayStub.Middleware;

public class StubInterceptor(RequestDelegate next, ILogger logger)
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!UrlValidator.ShouldProcessRequest(context.Request))
        {
            await next(context);
            return;
        }

        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();
        logger.Information("{CorrelationId} {HttpString}", correlationId, context.Request.HttpString());

        string? requestContent = null;
        try
        {
            context.Request.EnableBuffering();
            requestContent = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            logger.Information("{CorrelationId} {Content}", correlationId, requestContent);
        }
        catch (Exception ex)
        {
            logger.Warning(ex, "Unable to extract content from request");
        }

        if (IsDecisionComparerConflictRequest(context) || IsDecisionComparerSuccessRequest(context))
        {
            context.Response.StatusCode = IsDecisionComparerConflictRequest(context) ? (int)HttpStatusCode.Conflict : (int)HttpStatusCode.OK;
            context.Response.Headers.Date = DateTimeOffset.UtcNow.ToString("R");
            context.Response.Headers.Append("x-requested-path", new StringValues(context.Request.Path));

            var accept = context.Request.Headers.Accept.Count > 0 ? context.Request.Headers.Accept[0] : null;
            var contentType = accept ?? context.Request.ContentType ?? "";
            context.Response.ContentType = contentType;
            await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(IsDecisionComparerConflictRequest(context) ? string.Empty : requestContent ?? string.Empty)));
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.Headers.Date = DateTimeOffset.UtcNow.ToString("R");
            context.Response.Headers.Append("x-requested-path", new StringValues(context.Request.Path));

            var accept = context.Request.Headers.Accept.Count > 0 ? context.Request.Headers.Accept[0] : null;
            var contentType = accept ?? context.Request.ContentType ?? "";
            context.Response.ContentType = contentType;
            var content = contentType.StartsWith(MediaTypeNames.Application.Json) ? DefaultContent.ResponseJsonContent :
                contentType.StartsWith(MediaTypeNames.Application.Soap) ||
                contentType.StartsWith(MediaTypeNames.Application.Xml) ? DefaultContent.ResponseXmlContent :
                contentType.StartsWith(MediaTypeNames.Text.Plain) ? DefaultContent.ResponseTextContent : string.Empty;

            await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(content)));
        }
    }

    private bool IsDecisionComparerConflictRequest(HttpContext context)
    {
        return context.Request.Path.HasValue && (context.Request.Path.Value.StartsWith("/409/btms-decisions/") ||
                                                 context.Request.Path.Value.StartsWith("/409/alvs-decisions/") ||
                                                 context.Request.Path.Value.StartsWith("/409/btms-outbound-errors/") ||
                                                 context.Request.Path.Value.StartsWith("/409/alvs-outbound-errors/"));
    }

    private bool IsDecisionComparerSuccessRequest(HttpContext context)
    {
        return context.Request.Path.HasValue && (context.Request.Path.Value.StartsWith("/btms-decisions/") ||
                                                 context.Request.Path.Value.StartsWith("/alvs-decisions/") ||
                                                 context.Request.Path.Value.StartsWith("/btms-outbound-errors/") ||
                                                 context.Request.Path.Value.StartsWith("/alvs-outbound-errors/"));
    }
}