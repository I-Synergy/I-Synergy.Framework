using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace ISynergy.Filters
{
    public sealed class NoCacheFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Response != null)
                context.HttpContext.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue() { NoCache = true };
        }
    }
}