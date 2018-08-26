using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ISynergy.Exceptions
{
    public class ClaimNotFoundExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ClaimAuthorizationException)
                context.Result = new ChallengeResult();
        }
    }
}