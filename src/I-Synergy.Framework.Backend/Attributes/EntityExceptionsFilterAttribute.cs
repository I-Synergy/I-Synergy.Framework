using ISynergy.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ISynergy.Filters
{
    /// <summary>
    /// Filters and converts exceptions thrown by an EntityManager implementation to HTTPResponseExceptions.
    /// </summary>
    public class EntityExceptionsFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is EntityNotFoundException)
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}