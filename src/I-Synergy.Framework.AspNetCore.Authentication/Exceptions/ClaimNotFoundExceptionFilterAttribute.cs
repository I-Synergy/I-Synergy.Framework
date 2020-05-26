using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ISynergy.Framework.AspNetCore.Authentication.Exceptions
{
    /// <summary>
    /// Class ClaimNotFoundExceptionFilterAttribute. This class cannot be inherited.
    /// Implements the <see cref="ExceptionFilterAttribute" />
    /// </summary>
    /// <seealso cref="ExceptionFilterAttribute" />
    public sealed class ClaimNotFoundExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Called when [exception].
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ClaimAuthorizationException)
                context.Result = new ChallengeResult();
        }
    }
}
