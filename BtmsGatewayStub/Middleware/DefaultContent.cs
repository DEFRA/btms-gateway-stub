namespace BtmsGatewayStub.Middleware;

public static class DefaultContent
{
    public const string ResponseXmlContent = """
                                             <?xml version="1.0" encoding="utf-8"?>
                                             <soapenv:Envelope xmlns:soapenv="http://www.w3.org/2003/05/soap-envelope">
                                                 <soapenv:Body>
                                                     <ALVSClearanceResponse xmlns="http://submitimportdocumenthmrcfacade.types.esb.ws.cara.defra.com"
                                                                            xmlns:ns2="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"
                                                                            xmlns:ns3="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
                                                         <StatusCode>000</StatusCode>
                                                     </ALVSClearanceResponse>
                                                 </soapenv:Body>
                                             </soapenv:Envelope>
                                             """;

    public const string ResponseJsonContent = """
                                              {
                                                "StatusCode": "000"
                                              }
                                              """;

    public const string ResponseTextContent = "Working";
}