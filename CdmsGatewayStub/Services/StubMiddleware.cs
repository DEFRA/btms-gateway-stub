using System.Net;
using System.Net.Mime;
using System.Text;
using CdmsGatewayStub.Utils;
using Microsoft.Extensions.Primitives;
using ILogger = Serilog.ILogger;

namespace CdmsGatewayStub.Services;

public class StubMiddleware(RequestDelegate next, ILogger logger)
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Get && context.Request.Path.HasValue && context.Request.Path.Value.Trim('/').ToLower() == "health")
        {
            await next(context);
            return;
        }

        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();
        logger.Information("{CorrelationId} {HttpString}", correlationId, context.Request.HttpString());

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.Headers.Date = DateTimeOffset.UtcNow.ToString("R");
        context.Response.Headers.Append("x-requested-path", new StringValues(context.Request.Path));
        
        var contentType = context.Request.ContentType ?? "";
        context.Response.ContentType = contentType;
        var content = contentType.StartsWith(MediaTypeNames.Application.Json) ? ResponseJsonContent : 
                      contentType.StartsWith(MediaTypeNames.Application.Soap) || contentType.StartsWith(MediaTypeNames.Application.Xml) ? ResponseXmlContent : 
                      contentType.StartsWith(MediaTypeNames.Text.Plain) ? ResponseTextContent : string.Empty;
        
        await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(content)));
    }

    private const string ResponseXmlContent = """
<?xml version="1.0" encoding="utf-16" standalone="no"?>
<Envelope xmlns="http://www.w3.org/2003/05/soap-envelope/" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    <Body>
        <Response xmlns="http://example.com/"/>
    </Body>
</Envelope>
""";

    private const string ResponseJsonContent = """
{
  "Response": 0
}
""";

    private const string ResponseTextContent = """
Working
""";
}
