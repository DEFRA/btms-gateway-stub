using Microsoft.AspNetCore.Builder;

namespace CdmsGatewayStub.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   {
      var builder = WebApplication.CreateBuilder();

      var isDev = CdmsGatewayStub.Config.Environment.IsDevMode(builder);

      Assert.False(isDev);
   }
}
