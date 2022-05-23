using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Routing
{
    /// <summary>
    /// Class RouteDataRequestCultureProvider.
    /// Implements the <see cref="RequestCultureProvider" />
    /// </summary>
    /// <seealso cref="RequestCultureProvider" />
    public class RouteDataRequestCultureProvider : RequestCultureProvider
    {
        /// <summary>
        /// The zero result task
        /// </summary>
        private static readonly Task<ProviderCultureResult> ZeroResultTask = Task.FromResult(result: new ProviderCultureResult("en", "en"));

        /// <summary>
        /// Determines the provider culture result.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>Task&lt;ProviderCultureResult&gt;.</returns>
        /// <exception cref="ArgumentNullException">httpContext</exception>
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            Argument.IsNotNull(httpContext);

            // Test if length of result complies with ISO2 country code,
            // else return ZeroResultTask.
            var pathTest = httpContext.Request.Path.Value.Split('/')[1]?.ToString();

            if (pathTest is null || pathTest.Length != 2)
            {
                return ZeroResultTask;
            }
            
            string culture;
            var uiCulture = culture = pathTest;

            try
            {
                var providerResultCulture = new ProviderCultureResult(culture, uiCulture);
                return Task.FromResult(providerResultCulture);
            }
            catch (Exception)
            {
                return ZeroResultTask;
            }
        }
    }
}
