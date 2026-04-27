using System.Net;
using System.Text.Json;
using Library.Application.Exceptions;

namespace Library.Api.Middleware;

// Centralized error handling to capture exceptions and return JSON responses across API

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        // Handles 404: missing resources
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        // Handles 404: validation or logic errors
        catch (BadRequestException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        // Handles 409: state conflicts ~like duplicate entries 
        catch (ConflictException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.Conflict, ex.Message);
        }
        // Catch-all 500: unhandled system errors return a generic server-side error message
        catch (Exception ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");

            // Log detailed error information to server 
            Console.Error.WriteLine($"[{DateTime.UtcNow:u}] Unhandled exception: {ex}");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // Assignment-required format: { "error": "message here" }
        var response = new { error = message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
