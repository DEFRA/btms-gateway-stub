using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable once InconsistentNaming

namespace BtmsGatewayStub.Services.Simulation.Endpoints;

[ApiController, Route(Path)]
[Consumes(MediaTypeNames.Text.Plain), Produces(MediaTypeNames.Text.Plain)]
[SuppressMessage("SonarLint", "S101", Justification = "The class name appears on the swagger UI so want it recognizable there")]
public class CDS_Simulator(Simulator simulator) : ControllerBase
{   
    public const string Path = "cds-simulator";
    
    private const string ClearanceRequestToAlvsTargetPath = "/ITSW/CDS/SubmitImportDocumentCDSFacadeService";
    
    [HttpPost("clearance-request/to/alvs")]
    [SwaggerOperation(
        summary: "Simulates sending a Clearance Request SOAP message from CDS to ALVS",
        description: $"Routes to ALVS at https://t2.secure.services.defra.gsi.gov.uk{ClearanceRequestToAlvsTargetPath}")]
    public async Task<ActionResult> SendClearanceRequestToAlvs([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(ClearanceRequestToAlvsTargetPath, content, soapAction: "SubmitImportDocumentHMRCFacade");
    }
    
    private const string FinalisationNotificationToAlvsTargetPath = "/ITSW/CDS/NotifyFinalisedStateCDSFacadeService";
    
    [HttpPost("finalisation-notification/to/alvs")]
    [SwaggerOperation(
        summary: "Simulates sending a Finalisation Notification SOAP message from CDS to ALVS",
        description: $"Routes to ALVS at https://t2.secure.services.defra.gsi.gov.uk{FinalisationNotificationToAlvsTargetPath}")]
    public async Task<ActionResult> SendFinalisationNotificationToAlvs([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(FinalisationNotificationToAlvsTargetPath, content, soapAction: "NotifyFinalisedStateHMRCFacade");
    }
    
    private const string ErrorNotificationToAlvsTargetPath = "/ITSW/CDS/ALVSCDSErrorNotificationService";
    
    [HttpPost("error-notification/to/alvs")]
    [SwaggerOperation(
        summary: "Simulates sending a Error Notification SOAP message from CDS to ALVS",
        description: $"Routes to ALVS at https://t2.secure.services.defra.gsi.gov.uk{ErrorNotificationToAlvsTargetPath}")]
    public async Task<ActionResult> SendErrorNotificationToAlvs([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest(ErrorNotificationToAlvsTargetPath, content);
    }
}