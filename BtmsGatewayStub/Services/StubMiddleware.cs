using System.Net;
using System.Net.Mime;
using System.Text;
using BtmsGatewayStub.Utils;
using Microsoft.Extensions.Primitives;
using ILogger = Serilog.ILogger;

namespace BtmsGatewayStub.Services;

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

        try
        {
            context.Request.EnableBuffering();
            var requestContent = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            logger.Information("{CorrelationId} {Content}", correlationId, requestContent);
        }
        catch (Exception ex)
        {
            logger.Warning(ex, "Unable to extract content from request");
        }

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
<?xml version="1.0" encoding="utf-8"?>
<soapenv:Envelope xmlns:soapenv="http://www.w3.org/2003/05/soap-envelope">
    <soapenv:Body>
        <ALVSClearanceResponse xmlns="http://submitimportdocumenthmrcfacade.types.esb.ws.cara.defra.com"
                               xmlns:ns2="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"
                               xmlns:ns3="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
            <StatusCode>000</StatusCode>
        </ALVSClearanceResponse>
    </soapenv:Body>
</soapenv:Envelope>
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
