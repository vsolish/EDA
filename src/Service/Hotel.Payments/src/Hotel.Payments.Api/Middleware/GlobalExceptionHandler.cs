using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hotel.Payments.Api.Middleware;

public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = async context =>
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionFeature?.Error;

                if (exception == null) return;

                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalExceptionHandler");
                logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", Activity.Current?.Id);

                var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
                var isDevelopment = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment();

                var (statusCode, problemDetails) = exception switch
                {
                    FluentValidation.ValidationException ve => (StatusCodes.Status400BadRequest, CreateValidationProblemDetails(ve, traceId)),
                    _ => (StatusCodes.Status500InternalServerError, CreateProblemDetails(
                        "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        "An unexpected error occurred.",
                        StatusCodes.Status500InternalServerError,
                        isDevelopment ? exception.Message : null,
                        traceId))
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        });
        return app;
    }

    private static ProblemDetails CreateProblemDetails(string type, string title, int status, string? detail, string? traceId)
    {
        var pd = new ProblemDetails { Type = type, Title = title, Status = status, Detail = detail };
        if (traceId != null) pd.Extensions["traceId"] = traceId;
        return pd;
    }

    private static ProblemDetails CreateValidationProblemDetails(FluentValidation.ValidationException exception, string traceId)
    {
        var errors = exception.Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        var pd = CreateProblemDetails("https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "One or more validation errors occurred.", StatusCodes.Status400BadRequest, null, traceId);
        pd.Extensions["errors"] = errors;
        return pd;
    }
}