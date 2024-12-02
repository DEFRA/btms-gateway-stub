using Serilog;
using System.Diagnostics.CodeAnalysis;
using BtmsGatewayStub.Utils.Logging;
using BtmsGatewayStub.Services;
using BtmsGatewayStub.Utils;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog.Core;
using ILogger = Serilog.ILogger;

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

    builder.Services.AddOpenTelemetry()
        .WithMetrics(metrics =>
        {
            metrics.AddRuntimeInstrumentation()
                   .AddMeter(
                       "Microsoft.AspNetCore.Hosting",
                       "Microsoft.AspNetCore.Server.Kestrel",
                       "System.Net.Http");
        })
        .WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation();
        })
        .UseOtlpExporter();

    var logger = ConfigureLogging(builder);
    
    // Load certificates into Trust Store - Note must happen before Mongo and Http client connections
    builder.Services.AddCustomTrustStore(logger);

    ConfigureEndpoints(builder);
}

[ExcludeFromCodeCoverage]
static Logger ConfigureLogging(WebApplicationBuilder builder)
{
    builder.Logging.ClearProviders();
    var loggerConfiguration = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.With<LogLevelMapper>()
        .Enrich.WithProperty("service.version", Environment.GetEnvironmentVariable("SERVICE_VERSION"))
        .WriteTo.OpenTelemetry(options =>
        {
            options.LogsEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
            options.ResourceAttributes.Add("service.name", "btms-gateway-stub");
        });
    
    var logger = loggerConfiguration.CreateLogger();
    builder.Logging.AddSerilog(logger);
    builder.Services.AddSingleton<ILogger>(logger);
    logger.Information("Starting application");
    return logger;
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