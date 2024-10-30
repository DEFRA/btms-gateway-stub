using Microsoft.Extensions.Options;

namespace CdmsGatewayStub.Utils;

public static class Extensions
{
    public static string HttpString(this HttpRequest request) => $"{request.Method} {request.Scheme}://{request.Host}{request.Path}{request.QueryString} {request.Protocol} {request.ContentType}";

    public static WebApplicationBuilder ConfigureToType<T>(this WebApplicationBuilder builder, string sectionName) where T : class
    {
        builder.Services.Configure<T>(builder.Configuration.GetSection(sectionName));
        builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<T>>().Value);
        return builder;
    }
}