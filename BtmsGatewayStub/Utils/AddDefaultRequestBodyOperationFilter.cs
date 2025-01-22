using BtmsGatewayStub.Services;
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
            nameof(AlvsEndpoints.SendDecisionNotification) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "DecisionNotification.xml"))),
            
            _ => new OpenApiString(string.Empty)
        };

    }
}