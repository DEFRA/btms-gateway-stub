using System.Net.Mime;
using System.Text;
using BtmsGatewayStub.Utils.Http;
using Microsoft.AspNetCore.Mvc;

namespace BtmsGatewayStub.Services.Simulation;

public class Simulator(IHttpClientFactory httpClientFactory, IConfiguration configuration)
{
    public async Task<ObjectResult> SimulateSoapRequest(string urlPath, string content, string? soapAction = null, string contentType = MediaTypeNames.Application.Soap)
    {
        var client = httpClientFactory.CreateClient(Proxy.ProxyClient);
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"{configuration["gatewayUrl"]}{urlPath}");
        request.Content = new StringContent(content, Encoding.UTF8, contentType);
        if (soapAction != null) request.Headers.Add("SOAPAction", [soapAction]);
        
        var response = await client.SendAsync(request);
        
        return new ObjectResult(await response.Content.ReadAsStringAsync()) { StatusCode = (int)response.StatusCode };
    }
}