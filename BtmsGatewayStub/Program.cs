using Serilog;
using Serilog.Core;
using System.Diagnostics.CodeAnalysis;
using BtmsGatewayStub.Config;
using BtmsGatewayStub.Services;
using BtmsGatewayStub.Utils;
using BtmsGatewayStub.Utils.Logging;
using Microsoft.OpenApi.Models;
using Environment = System.Environment;

var app = CreateWebApplication(args);
await app.RunAsync();

[ExcludeFromCodeCoverage]
static WebApplication CreateWebApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureWebApplication(builder);

    ConfigureSwaggerBuilder(builder);

    var app = builder.Build();

    app.UseMiddleware<StubMiddleware>();
    app.MapHealthChecks("/health");
    // app.UseCheckRoutesEndpoints();

    ConfigureSwaggerApp(app);

    return app;
}

[ExcludeFromCodeCoverage]
static void ConfigureWebApplication(WebApplicationBuilder builder)
{
    builder.Configuration.AddEnvironmentVariables();
    builder.Configuration.AddIniFile("Properties/local.env", true);

    var logger = ConfigureLogging(builder);

    builder.Services.AddCustomTrustStore(logger);
    builder.Services.AddHealthChecks();
    builder.AddServices(logger);
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

[ExcludeFromCodeCoverage]
static void ConfigureSwaggerBuilder(WebApplicationBuilder builder)
{
    if (builder.IsSwaggerEnabled())
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("public-v0.1", new OpenApiInfo { Title = "TDM Public API", Version = "v1" }); });
    }
}

[ExcludeFromCodeCoverage]
static void ConfigureSwaggerApp(WebApplication app)
{
    if (app.IsSwaggerEnabled())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/public-v0.1/swagger.json", "public");
        });
    }
}