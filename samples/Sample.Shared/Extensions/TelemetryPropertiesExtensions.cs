﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;

namespace Sample.Extensions;
public static class TelemetryPropertiesExtensions
{
    public static void AddDefaultProperties(this IDictionary<string, object> properties, IInfoService infoService, IContext context)
    {
        Argument.IsNotNull(properties);
        Argument.IsNotNull(context);

        if (context.IsAuthenticated && context.Profile is { } profile)
        {
            properties.Add(nameof(profile.Username), profile.Username);
            properties.Add(nameof(profile.Email), profile.Email);
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

        properties.Add("host.name", infoService.HostName);
        properties.Add("host.ip", infoService.IPAddress);

        properties.Add("os.version", Environment.OSVersion.Version);
        properties.Add("os.architecture", Environment.Is64BitOperatingSystem ? "x64" : "x86");
        properties.Add("os.platform", Environment.OSVersion.Platform);
        properties.Add("os.clr", Environment.Version);

        properties.Add("session.id", Guid.NewGuid().ToString());
    }
}
