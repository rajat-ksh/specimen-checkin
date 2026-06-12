using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SpecimenCheckIn.Api.Infrastructure;

public class TenantHeaderOperationFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

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