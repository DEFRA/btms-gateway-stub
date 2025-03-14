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
    
    [HttpPost("decision-notification/to/hmrc")]
    [SwaggerOperation(
        summary: "Simulates sending a DecisionNotification SOAP message from ALVS to HMRC",
        description: $"Routes to CDS at https://syst32.hmrc.gov.uk{DecisionNotificationToCdsTargetPath}")]
    public async Task<ActionResult> SendDecisionNotificationToHmrc([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(DecisionNotificationToCdsTargetPath, content);
    }
    
    [HttpPost("error-notification/to/hmrc")]
    [SwaggerOperation(
        summary: "Simulates sending a HMRCErrorNotification SOAP message from ALVS to HMRC",
        description: $"Routes to CDS at https://syst32.hmrc.gov.uk{DecisionNotificationToCdsTargetPath}")]
    public async Task<ActionResult> SendErrorNotificationToHmrc([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(DecisionNotificationToCdsTargetPath, content);
    }
    
    private const string ClearanceRequestToIpaffsTargetPath = "/soapsearch/tst/sanco/traces_ws/sendALVSClearanceRequest";
    
    [HttpPost("clearance-request/to/ipaffs")]
    [SwaggerOperation(
        summary: "Simulates sending an ALVSClearanceRequest SOAP message from ALVS to IPAFFS",
        description: $"Routes to IPAFFS at https://importnotification-api-tst.azure.defra.cloud{ClearanceRequestToIpaffsTargetPath}")]
    public async Task<ActionResult> SendClearanceRequestToIpaffs([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(ClearanceRequestToIpaffsTargetPath, content, contentType: MediaTypeNames.Text.Xml);
    }
    
    private const string FinalisationNotificationToIpaffsTargetPath = "/soapsearch/tst/sanco/traces_ws/sendFinalisationNotificationRequest";
    
    [HttpPost("finalisation-notification/to/ipaffs")]
    [SwaggerOperation(
        summary: "Simulates sending a FinalisationNotificationRequest SOAP message from ALVS to IPAFFS",
        description: $"Routes to IPAFFS at https://importnotification-api-tst.azure.defra.cloud{FinalisationNotificationToIpaffsTargetPath}")]
    public async Task<ActionResult> SendFinalisationNotificationToIpaffs([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(FinalisationNotificationToIpaffsTargetPath, content, contentType: MediaTypeNames.Text.Xml);
    }
}