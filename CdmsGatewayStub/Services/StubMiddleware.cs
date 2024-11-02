using System.Net;
using System.Text;
using CdmsGatewayStub.Utils;
using ILogger = Serilog.ILogger;

namespace CdmsGatewayStub.Services;

public class StubMiddleware(RequestDelegate next, IStubActions stubActions, ILogger logger)
{
    private const string CorrelationIdName = "correlation-id";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Get && context.Request.Path.HasValue && context.Request.Path.Value.Trim('/').ToLower() == "health")
        {
            await next(context);
            return;
        }

        var correlationId = context.Request.Headers[CorrelationIdName];
        logger.Information("{CorrelationId} {HttpString}", correlationId, context.Request.HttpString());
        var delay = await stubActions.AddDelay();
        logger.Information("{CorrelationId} Delay for {DelayTotalMilliseconds} milliseconds", correlationId, delay.TotalMilliseconds);

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.ContentType = context.Request.ContentType;
        context.Response.Headers.Date = context.Request.Headers.Date;
        context.Response.Headers["Authorization"] = context.Request.Headers.Authorization;
        var content = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(ResponseContent));
        await context.Response.BodyWriter.WriteAsync(content);
    }

    private const string ResponseContent = """
<?xml version="1.0" encoding="utf-16" standalone="no"?>
<Envelope xmlns="http://www.w3.org/2003/05/soap-envelope/" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    <Body>
        <Response xmlns="http://example.com/"/>
    </Body>
</Envelope>
""";
}
