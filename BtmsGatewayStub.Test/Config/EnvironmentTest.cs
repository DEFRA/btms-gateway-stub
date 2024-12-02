using Microsoft.AspNetCore.Builder;

namespace BtmsGatewayStub.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   {
      var _builder = WebApplication.CreateBuilder();

      var isDev = BtmsGatewayStub.Config.Environment.IsDevMode(_builder);

      Assert.False(isDev);
   }
}
