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
            nameof(ALVS_Simulator.SendDecisionNotification) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "DecisionNotification.xml"))),
            nameof(CDS_Simulator.SendClearanceRequest) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "ClearanceRequest.xml"))),
            
            _ => new OpenApiString(string.Empty)
        };

    }
}