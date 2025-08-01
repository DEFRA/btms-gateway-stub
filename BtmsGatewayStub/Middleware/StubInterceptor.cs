using System.Diagnostics.CodeAnalysis;
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

    [SuppressMessage("SonarLint", "S3776", Justification = "This is test code")]
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
        else if (IsAlvsToCdsRequest(context))
        {
            context.Response.StatusCode = GetResponseStatusCode(requestContent);
            context.Response.Headers.Date = DateTimeOffset.UtcNow.ToString("R");
            context.Response.Headers.Append("x-requested-path", new StringValues(context.Request.Path));

            var accept = context.Request.Headers.Accept.Count > 0 ? context.Request.Headers.Accept[0] : null;
            var contentType = accept ?? context.Request.ContentType ?? "";
            context.Response.ContentType = contentType;
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.Headers.Date = DateTimeOffset.UtcNow.ToString("R");
            context.Response.Headers.Append("x-requested-path", new StringValues(context.Request.Path));

            var accept = context.Request.Headers.Accept.Count > 0 ? context.Request.Headers.Accept[0] : null;
            var contentType = accept ?? context.Request.ContentType ?? "";
            context.Response.ContentType = contentType;
            var content = GetContent(contentType);

            await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(content)));
        }
    }

    private static int GetResponseStatusCode(string? requestContent)
    {
        if (Contains503Request(requestContent))
        {
            return (int)HttpStatusCode.ServiceUnavailable;
        }
        
        if (Contains400Request(requestContent))
        {
            return (int)HttpStatusCode.BadRequest;
        }

        return (int)HttpStatusCode.NoContent;
    }
    
    private static bool Contains400Request(string? requestContent)
    {
        // Looks for a raw string containing the opening of the CorrelationId element with the value prefix of 400, whilst ignoring any namespacing in the element tag
        return requestContent?.Replace("&gt;", ">").Contains("CorrelationId>400") ?? false;
    }

    private static bool Contains503Request(string? requestContent)
    {
        // Looks for a raw string containing the opening of the CorrelationId element with the value prefix of 503, whilst ignoring any namespacing in the element tag
        return requestContent?.Replace("&gt;", ">").Contains("CorrelationId>503") ?? false;
    }
    
    private static bool IsAlvsToCdsRequest(HttpContext context)
    {
        return context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/ws/CDS/defra/alvsclearanceinbound/v1");
    }

    private static string GetContent(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return string.Empty;

        if (contentType.StartsWith(MediaTypeNames.Application.Json))
            return DefaultContent.ResponseJsonContent;

        if (contentType.StartsWith(MediaTypeNames.Application.Soap) ||
            contentType.StartsWith(MediaTypeNames.Application.Xml))
            return DefaultContent.ResponseXmlContent;

        if (contentType.StartsWith(MediaTypeNames.Text.Plain))
            return DefaultContent.ResponseTextContent;
        
        return string.Empty;
    }

    private static bool IsDecisionComparerConflictRequest(HttpContext context)
    {
        return context.Request.Path.HasValue && (context.Request.Path.Value.StartsWith("/409/btms-decisions/") ||
                                                 context.Request.Path.Value.StartsWith("/409/alvs-decisions/") ||
                                                 context.Request.Path.Value.StartsWith("/409/btms-outbound-errors/") ||
                                                 context.Request.Path.Value.StartsWith("/409/alvs-outbound-errors/"));
    }

    private static bool IsDecisionComparerSuccessRequest(HttpContext context)
    {
        return context.Request.Path.HasValue && (context.Request.Path.Value.StartsWith("/btms-decisions/") ||
                                                 context.Request.Path.Value.StartsWith("/alvs-decisions/") ||
                                                 context.Request.Path.Value.StartsWith("/btms-outbound-errors/") ||
                                                 context.Request.Path.Value.StartsWith("/alvs-outbound-errors/"));
    }
}