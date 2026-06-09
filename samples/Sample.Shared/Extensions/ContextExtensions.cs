using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using OpenTelemetry;

namespace Sample.Extensions;

public static class ContextExtensions
{
    public static void CreateBaggage(this IContext context, IInfoService infoService)
    {
        Argument.IsNotNull(context);
        Argument.IsNotNull(infoService);

        if (context.IsAuthenticated && context.Profile is { } profile)
        {

            Baggage.SetBaggage(nameof(profile.UserId), profile.UserId.ToString());
            Baggage.SetBaggage(nameof(profile.Name), profile.Name);
            Baggage.SetBaggage(nameof(profile.Email), profile.Email);
            Baggage.SetBaggage(nameof(profile.TenantId), profile.TenantId.ToString());

            Baggage.SetBaggage(nameof(profile.Expiration), profile.Expiration.ToString());
            Baggage.SetBaggage(nameof(profile.CountryCode), profile.CountryCode);
            Baggage.SetBaggage(nameof(profile.CultureCode), profile.CultureCode);
            Baggage.SetBaggage(nameof(profile.TimeZoneId), profile.TimeZoneId);
        }

        Baggage.SetBaggage(nameof(infoService.ProductName), infoService.ProductName);
        Baggage.SetBaggage(nameof(infoService.ProductVersion), infoService.ProductVersion.ToString());
    }
}
