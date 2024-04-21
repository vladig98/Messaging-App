using MessagingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingApp.Filters
{
    public class CheckLoggedInFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var error = new ErrorDetails()
                {
                    Message = "Already logged in!",
                    StatusCode = 400
                };

                context.Result = new BadRequestObjectResult(error);
            }
        }
    }
}
