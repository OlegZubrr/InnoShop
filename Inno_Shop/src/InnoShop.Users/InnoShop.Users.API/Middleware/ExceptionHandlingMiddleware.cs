using System.Net;
using System.Text.Json;
using InnoShop.Users.Domain.Exceptions;

namespace InnoShop.Users.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = ex switch
        {
            UserNotFoundException => (HttpStatusCode.NotFound, ex.Message),

            UserAlreadyExistsException => (HttpStatusCode.Conflict, ex.Message),

            InvalidCredentialsException => (HttpStatusCode.Unauthorized, ex.Message),
            UserDeactivatedException => (HttpStatusCode.Unauthorized, ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ex.Message),

            EmailNotConfirmedException => (HttpStatusCode.Forbidden, ex.Message),

            InvalidTokenException => (HttpStatusCode.BadRequest, ex.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
            ArgumentException => (HttpStatusCode.BadRequest, ex.Message),

            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
        else
            _logger.LogWarning("Client error occurred: {StatusCode} - {Message}", statusCode, message);

        var response = new
        {
            statusCode = (int)statusCode,
            message,
            type = ex.GetType().Name,
            stackTrace = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                ? ex.StackTrace
                : null
        };

        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}