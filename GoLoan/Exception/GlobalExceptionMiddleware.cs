using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GoLoan.Exceptions
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, System.Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                NotFoundException =>
                    ((int)HttpStatusCode.NotFound, exception.Message),

                KeyNotFoundException or FileNotFoundException =>
                    ((int)HttpStatusCode.NotFound, exception.Message),

                UnauthorizedException =>
                    ((int)HttpStatusCode.Unauthorized, exception.Message),

                UnauthorizedAccessException =>
                    ((int)HttpStatusCode.Unauthorized, "You are not authorized to access this resource."),

                BadRequestException =>
                    ((int)HttpStatusCode.BadRequest, exception.Message),

                BadHttpRequestException =>
                    ((int)HttpStatusCode.BadRequest, exception.Message),

                ArgumentNullException or ArgumentException or InvalidOperationException =>
                    ((int)HttpStatusCode.BadRequest, exception.Message),

                _ =>
                    ((int)HttpStatusCode.InternalServerError,
                     _env.IsDevelopment() ? exception.Message : "An unexpected server error occurred. Please try again later.")
            };

            context.Response.StatusCode = statusCode;

            var response = new ErrorResponse
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Details = _env.IsDevelopment() ? exception.StackTrace : null,
                Timestamp = DateTime.UtcNow
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }

    public class NotFoundException : System.Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class BadRequestException : System.Exception
    {
        public BadRequestException(string message) : base(message) { }
    }

    public class UnauthorizedException : System.Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
