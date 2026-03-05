using System.Net;
using System.Text.Json;
using FitnessAPI.Application.DTOs.Common;
using FitnessAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitnessAPI.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound, ex.Message, (IDictionary<string, string[]>?)null),
            UnauthorizedException ex => (HttpStatusCode.Unauthorized, ex.Message, null),
            BusinessException ex => (HttpStatusCode.BadRequest, ex.Message, null),
            ConflictException ex => (HttpStatusCode.Conflict, ex.Message, null),
            Domain.Exceptions.ValidationException ex => (HttpStatusCode.UnprocessableEntity, ex.Message, ex.Errors),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.", null)
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponseDto.Fail(message, errors);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
