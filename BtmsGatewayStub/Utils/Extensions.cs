using Microsoft.Extensions.Options;

namespace BtmsGatewayStub.Utils;

public static class Extensions
{
    public static string HttpString(this HttpRequest request) => $"{request.Protocol} {request.Method} {request.Scheme}://{request.Host}{request.Path}{request.QueryString} {request.ContentType}";

    public static WebApplicationBuilder ConfigureToType<T>(this WebApplicationBuilder builder, string sectionName) where T : class
    {
        builder.Services.Configure<T>(builder.Configuration.GetSection(sectionName));
        builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<T>>().Value);
        return builder;
    }
}