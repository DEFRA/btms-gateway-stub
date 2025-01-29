using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable once InconsistentNaming

namespace BtmsGatewayStub.Services.Simulation.Endpoints;

[ApiController, Route(Path)]
[SuppressMessage("SonarLint", "S101", Justification = "The class name appears on the swagger UI so want it recognizable there")]
public class CDS_Simulator(Simulator simulator) : ControllerBase
{   
    public const string Path = "cds-simulator";
    private const string TargetPath = "/ITSW/CDS/SubmitImportDocumentCDSFacadeService";
    
    [HttpPost("clearance-request")]
    [Consumes(MediaTypeNames.Text.Plain), Produces(MediaTypeNames.Text.Plain)]
    [SwaggerOperation(
        summary: "Simulates sending a Clearance Request SOAP message from CDS to ALVS",
        description: $"Routes to ALVS at https://t2.secure.services.defra.gsi.gov.uk{TargetPath}")]
    public async Task<ActionResult> SendClearanceRequestToAlvs([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest($"/cds{TargetPath}", content, soapAction: "SubmitImportDocumentHMRCFacadeOperation");
    }
}