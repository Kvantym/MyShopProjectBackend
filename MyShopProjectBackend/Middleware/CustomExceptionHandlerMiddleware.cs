using Azure;
using MyShopProjectBackend.Exceptions;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using ILogger = NLog.ILogger;

namespace MyShopProjectBackend.Middleware
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public CustomExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case BadRequestException badRequestException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = badRequestException.Message });
                    break;
                case NotFoundException notFoundException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new { error = notFoundException.Message });
                    break;
                case AuthorizationException authorizationException:
                    code = HttpStatusCode.Unauthorized;
                    result = JsonSerializer.Serialize(new { error = authorizationException.Message });
                    break;
                case RegistrationException registrationException:
                    code = HttpStatusCode.Conflict;
                    result = JsonSerializer.Serialize(new { error = registrationException.Message });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (string.IsNullOrEmpty(result))
            {
                result = JsonSerializer.Serialize(new { error = exception.Message });
            }
            return context.Response.WriteAsync(result);
        }
    }
}
