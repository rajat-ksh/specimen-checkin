namespace SpecimenCheckIn.Api.Infrastructure;

public class TenantMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        TenantContext tenantContext)
    {
        var header = context.Request.Headers["X-Lab-Id"]
            .FirstOrDefault();

        if (!Guid.TryParse(header, out var labId))
        {
            context.Response.StatusCode = 400;

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Missing X-Lab-Id header"
            });

            return;
        }

        tenantContext.LabId = labId;

        await _next(context);
    }
}