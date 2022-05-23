using ISynergy.Framework.Payment.Mollie.Abstractions.Services;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using System;

namespace ISynergy.Framework.Payment.Mollie.Services
{
    /// <summary>
    /// Class ValidatorService.
    /// Implements the <see cref="IValidatorService" />
    /// </summary>
    /// <seealso cref="IValidatorService" />
    public class ValidatorService : IValidatorService
    {
        /// <summary>
        /// Validates the URL link.
        /// </summary>
        /// <param name="urlObject">The URL object.</param>
        /// <exception cref="ArgumentException">Url object is null or href is empty: {urlObject}</exception>
        /// <exception cref="ArgumentException">Url does not point to the ISynergy.Framework.Payment.Mollie API: {urlObject.Href}</exception>
        /// <exception cref="ArgumentException">Url object is null or href is empty: {urlObject}</exception>
        public void ValidateUrlLink(UrlLink urlObject)
        {
            // Make sure the URL is not empty
            if (string.IsNullOrEmpty(urlObject?.Href))
            {
                throw new ArgumentException($"Url object is null or href is empty: {urlObject}");
            }

            // Don't execute any requests that don't point to the ISynergy.Framework.Payment.Mollie API URL for security reasons
            if (!urlObject.Href.Contains(Constants.ApiEndpoint))
            {
                throw new ArgumentException($"Url does not point to the ISynergy.Framework.Payment.Mollie API: {urlObject.Href}");
            }
        }
    }
}
