using System.Net.Mime;
using System.Text;
using BtmsGatewayStub.Utils.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BtmsGatewayStub.Services;

[ApiController, Route(Path)]
public class AlvsEndpoints(IHttpClientFactory httpClientFactory, IConfiguration configuration) : ControllerBase
{   
    public const string Path = "alvs-simulator";

    private readonly string _gatewayUrl = configuration["gatewayUrl"]!;
    
    [HttpPost("decision-notification")]
    [Consumes(MediaTypeNames.Text.Plain), Produces(MediaTypeNames.Text.Plain)]
    [SwaggerOperation(summary: "Simulates sending a Decision Notification SOAP message from ALVS to CDS at https://syst32.hmrc.gov.uk/ws/CDS/defra/alvsclearanceinbound/v1")]
    public async Task<ActionResult> SendDecisionNotification([FromBody] string content)
    {
        var client = httpClientFactory.CreateClient(Proxy.ProxyClient);
        var response = await client.PostAsync($"{_gatewayUrl}/alvs-cds/ws/CDS/defra/alvsclearanceinbound/v1", new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Soap));
        return new ObjectResult(await response.Content.ReadAsStringAsync()) { StatusCode = (int)response.StatusCode };
    }
}