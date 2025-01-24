using BtmsGatewayStub.Services.Simulation;
using BtmsGatewayStub.Services.Simulation.Endpoints;

namespace BtmsGatewayStub.Middleware;

public static class UrlValidator
{
    public static bool ShouldProcessRequest(HttpRequest request) => !(request.Path.Value is { } path
                                                                      && (path.StartsWith("/health", StringComparison.InvariantCultureIgnoreCase)
                                                                          || path.StartsWith("/swagger", StringComparison.InvariantCultureIgnoreCase)
                                                                          || path.StartsWith($"/{ALVS_Simulator.Path}", StringComparison.InvariantCultureIgnoreCase)
                                                                          || path.StartsWith($"/{CDS_Simulator.Path}", StringComparison.InvariantCultureIgnoreCase)));
}