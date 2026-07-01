using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SpecimenCheckIn.Api.Infrastructure;

public sealed class ExceptionHandlingMiddleware : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = GetStatusCode(exception);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = GetProblemType(statusCode),
            Title = GetTitle(exception, statusCode),
            Status = statusCode,
            Detail = exception is InvalidOperationException ? exception.Message : null,
            Instance = httpContext.Request.Path
        };

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken: cancellationToken);
        return true;
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        InvalidOperationException invalid when invalid.Message.Contains("not found", StringComparison.OrdinalIgnoreCase) => StatusCodes.Status404NotFound,
        InvalidOperationException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetProblemType(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "https://httpstatuses.com/400",
        StatusCodes.Status404NotFound => "https://httpstatuses.com/404",
        _ => "https://httpstatuses.com/500"
    };

    private static string GetTitle(Exception exception, int statusCode) => statusCode switch
    {
        StatusCodes.Status500InternalServerError => "An unexpected error occurred.",
        _ => exception.Message
    };
}
