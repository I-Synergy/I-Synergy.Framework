using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Threading.Tasks;

namespace ISynergy.Routing
{
    public class RouteDataRequestCultureProvider : RequestCultureProvider
    {
        private static readonly Task<ProviderCultureResult> ZeroResultTask = Task<ProviderCultureResult>.FromResult(result: new ProviderCultureResult("en", "en"));

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            string culture = null;
            string uiCulture = null;

            uiCulture = culture = httpContext.Request.Path.Value.Split('/')[1]?.ToString();

            if (culture is null)
            {
                return ZeroResultTask;
            }

            var providerResultCulture = new ProviderCultureResult(culture, uiCulture);

            return Task.FromResult(providerResultCulture);
        }
    }
}