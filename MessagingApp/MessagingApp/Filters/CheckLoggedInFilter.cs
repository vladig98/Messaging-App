using MessagingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagingApp.Filters
{
    public class CheckLoggedInFilter : ActionFilterAttribute
    {
        //check if the user is already logged in and prevents another log in
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (IsUserAuthenticated(context))
            {
                var error = new ErrorDetails()
                {
                    Message = "Already logged in!",
                    StatusCode = 400
                };

                context.Result = new BadRequestObjectResult(error);
            }
        }

        //check if the user is Authenticated
        private bool IsUserAuthenticated(ActionExecutingContext context)
        {
            return context.HttpContext.User.Identity.IsAuthenticated;
        }
     }
}
