using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Text.Json;

namespace ISynergy.Framework.OpenTelemetry.Processors;

public class UserContextEnrichingLogProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        var attributes = new List<KeyValuePair<string, object?>>();

        // Add user context information to all log entries
        var scopedContextService = ServiceLocator.Default.GetRequiredService<IScopedContextService>();
        var context = scopedContextService.GetRequiredService<IContext>();

        if (context is not null && context.Profile is not null)
        {
            attributes.Add(new KeyValuePair<string, object?>("Username", context.Profile.Username));
            attributes.Add(new KeyValuePair<string, object?>("UserId", context.Profile.UserId));
            attributes.Add(new KeyValuePair<string, object?>("AccountId", context.Profile.AccountId));
            attributes.Add(new KeyValuePair<string, object?>("Description", context.Profile.Description));
            attributes.Add(new KeyValuePair<string, object?>("Email", context.Profile.Email));
            attributes.Add(new KeyValuePair<string, object?>("CountryCode", context.Profile.CountryCode));
            attributes.Add(new KeyValuePair<string, object?>("CultureCode", context.Profile.CultureCode));
            attributes.Add(new KeyValuePair<string, object?>("TimeZoneId", context.Profile.TimeZoneId));
            attributes.Add(new KeyValuePair<string, object?>("Expration", context.Profile.Expiration));
            attributes.Add(new KeyValuePair<string, object?>("Modules", JsonSerializer.Serialize(context.Profile.Modules)));
            attributes.Add(new KeyValuePair<string, object?>("Roles", JsonSerializer.Serialize(context.Profile.Roles)));
        }

        // Add application context info
        var infoService = ServiceLocator.Default.GetRequiredService<IInfoService>();

        if (infoService is not null)
        {
            attributes.Add(new KeyValuePair<string, object?>("ProductName", infoService.ProductName));
            attributes.Add(new KeyValuePair<string, object?>("ProductVersion", infoService.ProductVersion));
        }

        data.Attributes = attributes;

        base.OnEnd(data);
    }
}
