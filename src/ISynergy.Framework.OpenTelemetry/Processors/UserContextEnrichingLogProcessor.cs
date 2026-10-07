using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Serialization;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Text.Json;

namespace ISynergy.Framework.OpenTelemetry.Processors;

/// <summary>
/// A log processor that enriches log records with user context information from the current scope.
/// </summary>
public class UserContextEnrichingLogProcessor : BaseProcessor<LogRecord>
{
    private readonly IContext _context;
    private readonly IInfoService _infoService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContextEnrichingLogProcessor"/> class.
    /// </summary>
    /// <param name="context">The user context.</param>
    /// <param name="infoService">The application info service.</param>
    public UserContextEnrichingLogProcessor(IContext context, IInfoService infoService)
    {
        _context = context;
        _infoService = infoService;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the current user's context — user id, name, email,
    /// tenant, organizations, culture, features and roles — is attached to every log record.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>. That context contains personally identifiable information and,
    /// together with <c>SendDefaultPii</c> on the exporter, would ship PII to every telemetry sink.
    /// Set it to <c>true</c> only where emitting that data is intended and covered by policy.
    /// </remarks>
    public bool IncludeUserContext { get; set; }

    /// <inheritdoc />
    public override void OnEnd(LogRecord data)
    {
        var attributes = new List<KeyValuePair<string, object?>>();

        // Add user context information to all log entries, only when explicitly opted in (PII).
        if (IncludeUserContext && _context.Profile is not null)
        {
            attributes.Add(new KeyValuePair<string, object?>("UserId", _context.Profile.UserId));
            attributes.Add(new KeyValuePair<string, object?>("TenantId", _context.Profile.TenantId));
            attributes.Add(new KeyValuePair<string, object?>("Name", _context.Profile.Name));
            attributes.Add(new KeyValuePair<string, object?>("Email", _context.Profile.Email));

            foreach (var organization in _context.Profile.Organizations)
            {
                attributes.Add(new KeyValuePair<string, object?>($"Organization_{organization.OrganizationId}", organization.Name));
            }

            attributes.Add(new KeyValuePair<string, object?>("CountryCode", _context.Profile.CountryCode));
            attributes.Add(new KeyValuePair<string, object?>("CultureCode", _context.Profile.CultureCode));
            attributes.Add(new KeyValuePair<string, object?>("TimeZoneId", _context.Profile.TimeZoneId));
            attributes.Add(new KeyValuePair<string, object?>("Expiration", _context.Profile.Expiration));
            attributes.Add(new KeyValuePair<string, object?>("Features", JsonSerializer.Serialize(_context.Profile.Features, TelemetryJsonContext.Default.ListString)));
            attributes.Add(new KeyValuePair<string, object?>("Roles", JsonSerializer.Serialize(_context.Profile.Roles, TelemetryJsonContext.Default.ListString)));
        }

        // Add application context info
        attributes.Add(new KeyValuePair<string, object?>("ProductName", _infoService.ProductName));
        attributes.Add(new KeyValuePair<string, object?>("ProductVersion", _infoService.ProductVersion));

        data.Attributes = attributes;

        base.OnEnd(data);
    }
}
