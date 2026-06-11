using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SpecimenCheckIn.Api.Infrastructure;

public class TenantHeaderOperationFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        operation.Parameters ??= [];

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "X-Lab-Id",
                In = ParameterLocation.Header,
                Required = true,
                Description = "Tenant Lab Id",
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
    }
}