using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable once InconsistentNaming

namespace BtmsGatewayStub.Services.Simulation.Endpoints;

[ApiController, Route(Path)]
[SuppressMessage("SonarLint", "S101", Justification = "The class name appears on the swagger UI so want it recognizable there")]
public class ALVS_Simulator(Simulator simulator) : ControllerBase
{   
    public const string Path = "alvs-simulator";
    private const string TargetPath = "/ws/CDS/defra/alvsclearanceinbound/v1";
    
    [HttpPost("decision-notification")]
    [Consumes(MediaTypeNames.Text.Plain), Produces(MediaTypeNames.Text.Plain)]
    [SwaggerOperation(
        summary: "Simulates sending a Decision Notification SOAP message from ALVS to CDS",
        description: $"Routes to CDS at https://syst32.hmrc.gov.uk{TargetPath}")]
    public async Task<ActionResult> SendDecisionNotification([FromBody] string content)
    {
        return await simulator.SimulateSoapRequest($"/alvs_cds{TargetPath}", content, "alvsclearanceinbound");
    }
}