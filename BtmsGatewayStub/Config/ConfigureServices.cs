using System.Diagnostics.CodeAnalysis;
using BtmsGatewayStub.Utils.Http;
using ILogger = Serilog.ILogger;

namespace BtmsGatewayStub.Config;

public static class ConfigureServices
{
    [ExcludeFromCodeCoverage]
    public static void AddServices(this WebApplicationBuilder builder, ILogger logger)
    {
        builder.Services.AddSingleton(logger);

        builder.Services.AddHttpProxyClient(logger); 
    }
}