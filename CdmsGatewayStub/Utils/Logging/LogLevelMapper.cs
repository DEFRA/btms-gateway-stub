using Serilog.Core;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;

namespace CdmsGatewayStub.Utils.Logging;

/**
 * Maps log levels from the C# default 'Information' etc to the node style 'info'.
 */
[ExcludeFromCodeCoverage]
public class LogLevelMapper : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var logLevel = logEvent.Level switch
        {
            LogEventLevel.Information => "info",
            LogEventLevel.Debug       => "debug",
            LogEventLevel.Error       => "error",
            LogEventLevel.Fatal       => "fatal",
            LogEventLevel.Warning     => "warn",
            _                         => "all"
        };

        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("log.level", logLevel));
    }
}
