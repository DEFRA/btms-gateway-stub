using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

/// <summary>
/// Endpoints that allow simulation of SOAP message interactions from ALVS to the BTMS Gateway.
/// Used for testing or stubbing message flows during integration development.
/// </summary>


namespace BtmsGatewayStub.Services.Simulation.Endpoints;

[ApiController, Route(Path)]
[Consumes(MediaTypeNames.Text.Plain), Produces(MediaTypeNames.Text.Plain)]
[SuppressMessage("SonarLint", "S101", Justification = "The class name appears on the swagger UI so want it recognizable there")]
public class ALVS_Simulator(Simulator simulator) : ControllerBase
{   
    public const string Path = "alvs-simulator";
    
    // Relative path for sending DecisionNotification messages to HMRC (CDS)
    private const string DecisionNotificationToCdsTargetPath = "/ws/CDS/defra/alvsclearanceinbound/v1";
    
    // Simulates sending a DecisionNotification SOAP message from ALVS to HMRC
    [HttpPost("decision-notification/to/hmrc")]
    [SwaggerOperation(
        summary: "Simulates sending a DecisionNotification SOAP message from ALVS to HMRC",
        description: $"Routes to CDS at https://syst32.hmrc.gov.uk{DecisionNotificationToCdsTargetPath}")]
    public async Task<ActionResult> SendDecisionNotificationToHmrc([FromBody] string content)
    {
        // Delegates the request to the simulator to handle or forward the SOAP message
        return await simulator.SimulateSoapRequest(DecisionNotificationToCdsTargetPath, content);
    }
    
    // Simulates sending a HMRCErrorNotification SOAP message from ALVS to HMRC
    [HttpPost("error-notification/to/hmrc")]
    [SwaggerOperation(
        summary: "Simulates sending a HMRCErrorNotification SOAP message from ALVS to HMRC",
        description: $"Routes to CDS at https://syst32.hmrc.gov.uk{DecisionNotificationToCdsTargetPath}")]
    public async Task<ActionResult> SendErrorNotificationToHmrc([FromBody] string content)
    {
        // Delegates the request to the simulator to handle or forward the SOAP message
        return await simulator.SimulateSoapRequest(DecisionNotificationToCdsTargetPath, content);
    }
    
    // Relative path for sending ClearanceRequest messages to IPAFFS
    private const string ClearanceRequestToIpaffsTargetPath = "/soapsearch/pre/sanco/traces_ws/sendALVSClearanceRequest";
    
    // Simulates sending an ALVSClearanceRequest SOAP message from ALVS to IPAFFS
    [HttpPost("clearance-request/to/ipaffs")]
    [SwaggerOperation(
        summary: "Simulates sending an ALVSClearanceRequest SOAP message from ALVS to IPAFFS",
        description: $"Routes to IPAFFS at https://importnotification-api-pre.azure.defra.cloud{ClearanceRequestToIpaffsTargetPath}")]
    public async Task<ActionResult> SendClearanceRequestToIpaffs([FromBody] string content)
    {
        // Delegates the request to the simulator to handle or forward the SOAP message
        return await simulator.SimulateSoapRequest(ClearanceRequestToIpaffsTargetPath, content, contentType: MediaTypeNames.Text.Xml);
    }
    
    // Relative path for sending FinalisationNotification messages to IPAFFS
    private const string FinalisationNotificationToIpaffsTargetPath = "/soapsearch/pre/sanco/traces_ws/sendFinalisationNotificationRequest";
    
    // Simulates sending a FinalisationNotificationRequest SOAP message from ALVS to IPAFFS
    [HttpPost("finalisation-notification/to/ipaffs")]
    [SwaggerOperation(
        summary: "Simulates sending a FinalisationNotificationRequest SOAP message from ALVS to IPAFFS",
        description: $"Routes to IPAFFS at https://importnotification-api-pre.azure.defra.cloud{FinalisationNotificationToIpaffsTargetPath}")]
    public async Task<ActionResult> SendFinalisationNotificationToIpaffs([FromBody] string content)
    {
        // Delegates the request to the simulator to handle or forward the SOAP message
        return await simulator.SimulateSoapRequest(FinalisationNotificationToIpaffsTargetPath, content, contentType: MediaTypeNames.Text.Xml);
    }
    
    // Relative path for sending DecisionNotification messages to IPAFFS
    private const string DecisionNotificationToIpaffsTargetPath = "/soapsearch/pre/sanco/traces_ws/sendALVSDecisionNotification";
    
    // Simulates sending an ALVSDecisionNotification SOAP message from ALVS to IPAFFS
    [HttpPost("decision-notification/to/ipaffs")]
    [SwaggerOperation(
        summary: "Simulates sending an ALVSDecisionNotification SOAP message from ALVS to IPAFFS",
        description: $"Routes to IPAFFS at https://importnotification-api-pre.azure.defra.cloud{DecisionNotificationToIpaffsTargetPath}")]
    public async Task<ActionResult> SendDecisionNotificationToIpaffs([FromBody] string content)
    {
        // Delegates the request to the simulator to handle or forward the SOAP message
        return await simulator.SimulateSoapRequest(DecisionNotificationToIpaffsTargetPath, content, contentType: MediaTypeNames.Text.Xml);
    }
    
    // Relative path for sending SearchCertificate requests to IPAFFS
    private const string SearchCertificateToIpaffsTargetPath = "/soapsearch/pre/sanco/traces_ws/searchCertificate";
    
    // Simulates sending a SearchCertificate SOAP message from ALVS to IPAFFS
    [HttpPost("search-certificate/to/ipaffs")]
    [SwaggerOperation(
        summary: "Simulates sending a SearchCertificate SOAP message from ALVS to IPAFFS",
        description: $"Routes to IPAFFS at https://importnotification-api-pre.azure.defra.cloud{SearchCertificateToIpaffsTargetPath}")]
    public async Task<ActionResult> SendSearchCertificateToIpaffs([FromBody] string content)
    {
        // Delegates the request to the simulator to handle or forward the SOAP message
        return await simulator.SimulateSoapRequest(SearchCertificateToIpaffsTargetPath, content, contentType: MediaTypeNames.Text.Xml);
    }
    
    // Relative path for polling SearchCertificate results from IPAFFS
    private const string PollSearchCertificateResultToIpaffsTargetPath = "/soapsearch/pre/sanco/traces_ws/pollSearchCertificateResult";
    
    // Simulates sending a PollSearchCertificateResult SOAP message from ALVS to IPAFFS
    [HttpPost("poll-search-certificate-result/to/ipaffs")]
    [SwaggerOperation(
        summary: "Simulates sending a PollSearchCertificateResult SOAP message from ALVS to IPAFFS",
        description: $"Routes to IPAFFS at https://importnotification-api-pre.azure.defra.cloud{PollSearchCertificateResultToIpaffsTargetPath}")]
    public async Task<ActionResult> SendPollSearchCertificateResultToIpaffs([FromBody] string content)
    {
        // Delegates the request to the simulator to handle or forward the SOAP message
        return await simulator.SimulateSoapRequest(PollSearchCertificateResultToIpaffsTargetPath, content, contentType: MediaTypeNames.Text.Xml);
    }
    
}