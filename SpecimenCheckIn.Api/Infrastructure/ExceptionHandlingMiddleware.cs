using System.Net;
using System.Text.Json;

namespace SpecimenCheckIn.Api.Infrastructure;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is InvalidOperationException)
        {
            var msg = exception.Message ?? "Invalid operation";
            if (msg.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = msg }));
            }

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = msg }));
        }

        // unexpected
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var problem = new { error = "An unexpected error occurred." };
        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
