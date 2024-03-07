using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthDemo.Web.Configuration.Swagger
{
    public class SwaggerRefreshTokenEndpointFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "refresh_token",
                In = ParameterLocation.Query,
                Description = "Refresh Token",
                Required = true, // Depending on your requirement
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }

}
