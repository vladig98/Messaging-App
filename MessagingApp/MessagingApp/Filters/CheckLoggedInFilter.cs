using MessagingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingApp.Filters
{
    public class CheckLoggedInFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsUserAuthenticated(context))
            {
                return;
            }

            ErrorDetails error = new()
            {
                Message = "Already logged in!",
                StatusCode = 400
            };

            context.Result = new BadRequestObjectResult(error);
        }

        private static bool IsUserAuthenticated(ActionExecutingContext context)
        {
            return context.HttpContext.User.Identity?.IsAuthenticated ?? false;
        }
     }
}
