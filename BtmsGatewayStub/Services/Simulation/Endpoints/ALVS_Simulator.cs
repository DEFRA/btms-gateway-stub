using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable once InconsistentNaming

namespace BtmsGatewayStub.Services.Simulation.Endpoints;

[ApiController, Route(Path)]
[Consumes(MediaTypeNames.Text.Plain), Produces(MediaTypeNames.Text.Plain)]
[SuppressMessage("SonarLint", "S101", Justification = "The class name appears on the swagger UI so want it recognizable there")]
public class ALVS_Simulator(Simulator simulator) : ControllerBase
{   
    public const string Path = "alvs-simulator";
    
    private const string DecisionNotificationToCdsTargetPath = "/ws/CDS/defra/alvsclearanceinbound/v1";
    
    [HttpPost("decision-notification/to/cds")]
    [SwaggerOperation(
        summary: "Simulates sending a Decision Notification SOAP message from ALVS to CDS",
        description: $"Routes to CDS at https://syst32.hmrc.gov.uk{DecisionNotificationToCdsTargetPath}")]
    public async Task<ActionResult> SendDecisionNotificationToCds([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(DecisionNotificationToCdsTargetPath, content);
    }
    
    private const string ErrorNotificationToCdsTargetPath = "/prsup/PRRestService/ALVS/Service/DecisionNotification";
    
    [HttpPost("error-notification/to/cds")]
    [SwaggerOperation(
        summary: "Simulates sending a Error Notification SOAP message from ALVS to CDS",
        description: $"Routes to CDS at https://syst32.hmrc.gov.uk{ErrorNotificationToCdsTargetPath}")]
    public async Task<ActionResult> SendErrorNotificationToCds([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(ErrorNotificationToCdsTargetPath, content);
    }
    
    private const string ClearanceRequestToIpaffsTargetPath = "/soapsearch/vnet/sanco/traces_ws/sendALVSClearanceRequest";
    
    [HttpPost("clearance-request/to/ipaffs")]
    [SwaggerOperation(
        summary: "Simulates sending a Clearance Request SOAP message from ALVS to IPAFFS",
        description: $"Routes to IPAFFS at https://importnotification-api-static-snd.azure.defra.cloud{ClearanceRequestToIpaffsTargetPath}")]
    public async Task<ActionResult> SendClearanceRequestToIpaffs([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(ClearanceRequestToIpaffsTargetPath, content, contentType: MediaTypeNames.Text.Xml);
    }
    
    private const string FinalisationNotificationToIpaffsTargetPath = "/soapsearch/vnet/sanco/traces_ws/sendFinalisationNotificationRequest";
    
    [HttpPost("finalisation-notification/to/ipaffs")]
    [SwaggerOperation(
        summary: "Simulates sending a Finalisation Notification SOAP message from ALVS to IPAFFS",
        description: $"Routes to IPAFFS at https://importnotification-api-static-snd.azure.defra.cloud{FinalisationNotificationToIpaffsTargetPath}")]
    public async Task<ActionResult> SendFinalisationNotificationToIpaffs([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(FinalisationNotificationToIpaffsTargetPath, content, contentType: MediaTypeNames.Text.Xml);
    }
}