using MessagingApp.Models;

namespace MessagingApp.Middleware
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            ErrorDetails error = new()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error"
            };

            _logger.LogError(ex, "{Message}", ex.Message);

            if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, "{Message}", ex.InnerException.Message);
            }

            await context.Response.WriteAsync(error.ToString());
        }
    }
}
