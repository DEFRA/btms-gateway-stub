using System.Net.Mime;
using System.Text;
using BtmsGatewayStub.Utils.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
// ReSharper disable once InconsistentNaming

namespace BtmsGatewayStub.Services;

[ApiController, Route(Path)]
public class CDS_Simulator(IHttpClientFactory httpClientFactory, IConfiguration configuration) : ControllerBase
{   
    public const string Path = "cds-simulator";
    private const string TargetPath = "/ITSW/CDS/SubmitImportDocumentCDSFacadeService";

    private readonly string _gatewayUrl = configuration["gatewayUrl"]!;
    
    [HttpPost("clearance-request")]
    [Consumes(MediaTypeNames.Text.Plain), Produces(MediaTypeNames.Text.Plain)]
    [SwaggerOperation(
        summary: "Simulates sending a Clearance Request SOAP message from CDS to ALVS",
        description: $"Routes to ALVS at https://t2.secure.services.defra.gsi.gov.uk{TargetPath}")]
    public async Task<ActionResult> SendClearanceRequest([FromBody] string content)
    {
        var client = httpClientFactory.CreateClient(Proxy.ProxyClient);
        var response = await client.PostAsync($"{_gatewayUrl}/cds{TargetPath}", new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Soap));
        return new ObjectResult(await response.Content.ReadAsStringAsync()) { StatusCode = (int)response.StatusCode };
    }
}