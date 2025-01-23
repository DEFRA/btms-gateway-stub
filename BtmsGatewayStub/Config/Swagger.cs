using System.Diagnostics.CodeAnalysis;
using BtmsGatewayStub.Utils;
using Microsoft.OpenApi.Models;

namespace BtmsGatewayStub.Config;


[ExcludeFromCodeCoverage]
public static class Swagger
{
    public static void ConfigureSwaggerBuilder(this WebApplicationBuilder builder)
    {
        if (builder.IsSwaggerEnabled())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("public-v0.1", new OpenApiInfo { Title = "Public API", Version = "v1" });
                c.EnableAnnotations();
                c.OperationFilter<AddDefaultRequestBodyOperationFilter>();
            });
        }
    }

    public static void ConfigureSwaggerApp(this WebApplication app)
    {
        if (app.IsSwaggerEnabled())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/public-v0.1/swagger.json", "public");
                c.DisplayRequestDuration();
            });
        }
    }
    
    private static bool IsSwaggerEnabled(this WebApplicationBuilder builder) => builder.IsDevMode() || builder.Configuration.GetValue<bool>("EnableSwagger");
    
    private static bool IsSwaggerEnabled(this WebApplication app) => app.IsDevMode() || app.Configuration.GetValue<bool>("EnableSwagger");
}