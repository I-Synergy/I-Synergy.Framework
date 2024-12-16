using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using Microsoft.ApplicationInsights.DataContracts;

namespace ISynergy.Framework.Logging.Extensions;

public static class TelemetryPropertiesExtensions
{
    public static void AddDefaultProperties(this IDictionary<string, string> properties, TelemetryContext telemetryContext, IContext context, IInfoService infoService)
    {
        Argument.IsNotNull(properties);
        Argument.IsNotNull(context);
        Argument.IsNotNull(infoService);

        SetUserProfile(telemetryContext, context.IsAuthenticated, context.Profile);

        if (context.IsAuthenticated && context.Profile is { } profile)
        {
            properties.Add(nameof(profile.Username), profile.Username);
            properties.Add(nameof(profile.UserId), profile.UserId.ToString());

            properties.Add(nameof(profile.AccountId), profile.AccountId.ToString());
            properties.Add(nameof(profile.AccountDescription), profile.AccountDescription);
            properties.Add(nameof(profile.LicenseExpration), profile.LicenseExpration.ToString());
            properties.Add(nameof(profile.LicenseUsers), profile.LicenseUsers.ToString());

            properties.Add(nameof(profile.CountryCode), profile.CountryCode);
            properties.Add(nameof(profile.TimeZoneId), profile.TimeZoneId);
        }

        properties.Add(nameof(infoService.ProductName), infoService.ProductName);
        properties.Add(nameof(infoService.ProductVersion), infoService.ProductVersion.ToString());
    }

    /// <summary>
    /// Sets profile in logging context.
    /// </summary>
    private static void SetUserProfile(TelemetryContext telemetryContext, bool isAuthenticated, IProfile profile)
    {
        if (isAuthenticated && profile is { } currentProfile)
        {
            telemetryContext.User.Id = currentProfile.Email;
            telemetryContext.User.AuthenticatedUserId = currentProfile.UserId.ToString();
            telemetryContext.User.AccountId = currentProfile.AccountDescription;
        }
        else
        {
            telemetryContext.User.Id = string.Empty;
            telemetryContext.User.AuthenticatedUserId = string.Empty;
            telemetryContext.User.AccountId = string.Empty;
        }
    }
}
