using Microsoft.AspNetCore.Builder;

namespace BtmsGatewayStub.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   {
      var builder = WebApplication.CreateBuilder();

      var isDev = BtmsGatewayStub.Config.Environment.IsDevMode(builder);

      Assert.False(isDev);
   }
}
