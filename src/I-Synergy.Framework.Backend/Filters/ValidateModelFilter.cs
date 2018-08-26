using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ISynergy.Filters
{
    /// <summary>
    /// Checks if ModelState is valid.
    /// If the requirements are not met, bad request is set as result.
    /// </summary>
    public class ValidateModelFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if model is valid, with build-in modelstate validators.
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}