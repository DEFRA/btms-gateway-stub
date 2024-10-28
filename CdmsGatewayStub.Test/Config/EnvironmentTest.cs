using Microsoft.AspNetCore.Builder;

namespace CdmsGatewayStub.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   {
      var _builder = WebApplication.CreateBuilder();

      var isDev = CdmsGatewayStub.Config.Environment.IsDevMode(_builder);

      Assert.False(isDev);
   }
}
