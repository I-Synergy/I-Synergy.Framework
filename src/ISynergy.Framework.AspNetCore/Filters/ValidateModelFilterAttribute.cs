namespace ISynergy.Framework.AspNetCore.Filters
{
    /// <summary>
    /// Checks if ModelState is valid.
    /// If the requirements are not met, bad request is set as result.
    /// </summary>
    public sealed class ValidateModelFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called when [action executing].
        /// </summary>
        /// <param name="context">The context.</param>
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
