using MessagingApp.Models;

namespace MessagingApp.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        //main method that invokes the middleware
        public async Task Invoke(HttpContext context)
        {
            try
            {
                //pass the context to the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                //log any exception that may occur
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        //handle any exception that may occur
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            //returns a 500 error on a failure
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error"
            }.ToString());
        }
    }
}
