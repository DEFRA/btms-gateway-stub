using Serilog;
using Serilog.Core;
using System.Diagnostics.CodeAnalysis;
using BtmsGatewayStub.Config;
using BtmsGatewayStub.Middleware;
using BtmsGatewayStub.Services.Simulation;
using BtmsGatewayStub.Utils;
using BtmsGatewayStub.Utils.Http;
using BtmsGatewayStub.Utils.Logging;
using Environment = System.Environment;
using ILogger = Serilog.ILogger;

var app = CreateWebApplication(args);
await app.RunAsync();

[ExcludeFromCodeCoverage]
static WebApplication CreateWebApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureWebApplication(builder);

    builder.ConfigureSwaggerBuilder();

    var app = builder.Build();

    app.UseMiddleware<StubInterceptor>();
    app.MapHealthChecks("/health");
    app.UseMvc();
    app.ConfigureSwaggerApp();

    return app;
}

[ExcludeFromCodeCoverage]
static void ConfigureWebApplication(WebApplicationBuilder builder)
{
    builder.Configuration.AddEnvironmentVariables();
    builder.Configuration.AddIniFile("Properties/local.env", true);

    var logger = ConfigureLogging(builder);

    builder.Services.AddControllers(options =>
    {
        options.EnableEndpointRouting = false;
        options.InputFormatters.Add(new PlainTextFormatter());
    });
    builder.Services.AddCustomTrustStore(logger);
    builder.Services.AddHealthChecks();
    builder.Services.AddSingleton<ILogger>(logger);
    builder.Services.AddHttpProxyClient(logger);
    builder.Services.AddScoped<Simulator>();
}

[ExcludeFromCodeCoverage]
static Logger ConfigureLogging(WebApplicationBuilder builder)
{
    builder.Logging.ClearProviders();
    var loggerConfiguration = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.With<LogLevelMapper>()
        .Enrich.WithProperty("service.version", Environment.GetEnvironmentVariable("SERVICE_VERSION"));
    
    var logger = loggerConfiguration.CreateLogger();
    builder.Logging.AddSerilog(logger);
    logger.Information("Starting application");
    return logger;
}
