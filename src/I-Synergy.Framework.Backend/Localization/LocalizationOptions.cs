using ISynergy.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ISynergy.Localization
{
    public static class LocalizationOptions
    {
        public static RequestLocalizationOptions BuildLocalizationOptions()
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en"),
                new CultureInfo("nl")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: "nl", uiCulture: "nl"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            options.RequestCultureProviders.Clear();
            options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
            options.RequestCultureProviders.Insert(1, new QueryStringRequestCultureProvider());
            options.RequestCultureProviders.Insert(2, new CookieRequestCultureProvider());
            options.RequestCultureProviders.Insert(3, new AcceptLanguageHeaderRequestCultureProvider());

            return options;
        }
    }
}
