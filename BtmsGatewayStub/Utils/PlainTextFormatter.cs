using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace BtmsGatewayStub.Utils;

public class PlainTextFormatter : TextInputFormatter
{
    public PlainTextFormatter()
    {
        SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaTypeNames.Text.Plain));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
        SupportedEncodings.Add(Encoding.ASCII);
    }

    protected override bool CanReadType(Type type) => type == typeof(string);

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        var reader = new StreamReader(context.HttpContext.Request.Body);
        var plainText = await reader.ReadToEndAsync();
        
        return await InputFormatterResult.SuccessAsync(plainText);
    }
}