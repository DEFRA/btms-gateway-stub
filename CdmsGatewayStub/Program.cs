using Serilog;
using System.Diagnostics.CodeAnalysis;
using CdmsGatewayStub.Utils.Logging;
using CdmsGatewayStub.Services;
using CdmsGatewayStub.Utils;

//-------- Configure the WebApplication builder------------------//

var app = CreateWebApplication(args);
await app.RunAsync();

[ExcludeFromCodeCoverage]
static WebApplication CreateWebApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureWebApplication(builder);

    var app = BuildWebApplication(builder);

    return app;
}

[ExcludeFromCodeCoverage]
static void ConfigureWebApplication(WebApplicationBuilder builder)
{
    builder.Configuration.AddEnvironmentVariables();
    builder.Configuration.AddIniFile("Properties/local.env", true);

    builder.ConfigureToType<StubDelaysConfig>("StubDelays");

    builder.Services.AddSingleton<IStubActions, StubActions>();

    ConfigureLogging(builder);

    ConfigureEndpoints(builder);
}

[ExcludeFromCodeCoverage]
static void ConfigureLogging(WebApplicationBuilder builder)
{
    builder.Logging.ClearProviders();
    var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.With<LogLevelMapper>()
        .CreateLogger();
    builder.Logging.AddSerilog(logger);
    logger.Information("Starting application");
}

[ExcludeFromCodeCoverage]
static void ConfigureEndpoints(WebApplicationBuilder builder)
{
    builder.Services.AddHealthChecks();
}

[ExcludeFromCodeCoverage]
static WebApplication BuildWebApplication(WebApplicationBuilder builder)
{
    var app = builder.Build();

    app.UseMiddleware<StubMiddleware>();
   
    app.MapHealthChecks("/health");

    return app;
}