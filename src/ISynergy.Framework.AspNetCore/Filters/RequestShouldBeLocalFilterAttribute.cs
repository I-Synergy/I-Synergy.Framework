using ISynergy.Framework.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ISynergy.Framework.AspNetCore.Filters
{
    /// <summary>
    /// Class RequestShouldBeLocalFilter. This class cannot be inherited.
    /// Implements the <see cref="ActionFilterAttribute" />
    /// </summary>
    /// <seealso cref="ActionFilterAttribute" />
    public sealed class RequestShouldBeLocalFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called when [action executing].
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.IsLocal())
                context.Result = new ForbidResult();
        }
    }
}
