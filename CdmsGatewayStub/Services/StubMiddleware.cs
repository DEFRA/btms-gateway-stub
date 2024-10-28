using System.Net;
using System.Net.Mime;
using System.Text;
using CdmsGatewayStub.Utils;

namespace CdmsGatewayStub.Services;

public class StubMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method != HttpMethod.Post.ToString())
        {
            await next(context);
            return;
        }

        Console.WriteLine($"{context.Request.Headers["x-correlation-id"]} {context.Request.HttpString()}");

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.ContentType = MediaTypeNames.Application.Soap;
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
