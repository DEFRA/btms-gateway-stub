using BtmsGatewayStub.Services.Simulation;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BtmsGatewayStub.Utils;

public class AddDefaultRequestBodyOperationFilter : IOperationFilter
{
    private const string ExamplesPath = "Examples";
    
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