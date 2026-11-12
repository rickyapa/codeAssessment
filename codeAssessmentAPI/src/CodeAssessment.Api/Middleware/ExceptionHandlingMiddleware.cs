using CodeAssessment.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CodeAssessment.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Validation failed", exception.Errors.Select(error => error.ErrorMessage));
        }
        catch (ProductDomainException exception)
        {
            await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Product rule violated", [exception.Message]);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblemDetailsAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred", ["The server could not complete the request."]);
        }
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, IEnumerable<string> errors)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(errors
            .GroupBy(error => error)
            .ToDictionary(group => group.Key, group => group.ToArray()))
        {
            Status = statusCode,
            Title = title,
            Instance = context.Request.Path
        });
    }
}
