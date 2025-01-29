using BtmsGatewayStub.Services.Simulation.Endpoints;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BtmsGatewayStub.Services.Simulation;

public class AddDefaultExamplesToSimulatorsInSwagger : IOperationFilter
{
    private static readonly string ExamplesPath = Path.Combine("Services", "Simulation", "Examples");
    
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody == null || !operation.RequestBody.Content.Any()) return;
        
        operation.RequestBody.Content["text/plain"].Example = context.MethodInfo.Name switch
        {
            nameof(ALVS_Simulator.SendDecisionNotificationToCds) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "AlvsToCdsDecisionNotification.xml"))),
            nameof(ALVS_Simulator.SendClearanceRequestToIpaffs) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "AlvsToIpaffsClearanceRequest.xml"))),
            nameof(ALVS_Simulator.SendFinalisationNotificationToIpaffs) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "AlvsToIpaffsFinalisationNotification.xml"))),
            nameof(CDS_Simulator.SendClearanceRequestToAlvs) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "CdsToAlvsClearanceRequest.xml"))),
            nameof(CDS_Simulator.SendFinalisationNotificationToAlvs) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "CdsToAlvsFinalisationNotification.xml"))),
            
            _ => new OpenApiString(string.Empty)
        };

    }
}