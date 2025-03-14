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
            nameof(HMRC_Simulator.SendClearanceRequestToAlvs) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "ALVSClearanceRequest_HmrcToAlvs.xml"))),
            nameof(HMRC_Simulator.SendFinalisationNotificationToAlvs) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "FinalisationNotificationRequest_HmrcToAlvs.xml"))),
            nameof(HMRC_Simulator.SendErrorNotificationToAlvs) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "ALVSErrorNotificationRequest_HmrcToAlvs.xml"))),
            nameof(ALVS_Simulator.SendDecisionNotificationToHmrc) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "DecisionNotification_AlvsToHmrc.xml"))),
            nameof(ALVS_Simulator.SendErrorNotificationToHmrc) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "HMRCErrorNotification_AlvsToHmrc.xml"))),
            nameof(ALVS_Simulator.SendClearanceRequestToIpaffs) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "ALVSClearanceRequest_AlvsToIpaffs.xml"))),
            nameof(ALVS_Simulator.SendFinalisationNotificationToIpaffs) => new OpenApiString(File.ReadAllText(Path.Combine(ExamplesPath, "FinalisationNotificationRequest_AlvsToIpaffs.xml"))),
            
            _ => new OpenApiString(string.Empty)
        };

    }
}