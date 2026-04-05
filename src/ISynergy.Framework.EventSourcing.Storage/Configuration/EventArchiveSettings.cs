namespace ISynergy.Framework.EventSourcing.Storage.Configuration;

/// <summary>
/// Configuration options for the event archive job.
/// Bind from <c>appsettings.json</c> section <c>EventArchive</c>.
/// </summary>
public sealed class EventArchiveSettings
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "EventArchive";

    /// <summary>
    /// Events older than this many days are eligible for archiving.
    /// Set to 0 to archive all events immediately (useful for demos/testing).
    /// Default: 180 days.
    /// </summary>
    public int RetentionDays { get; set; } = 180;

    /// <summary>
    /// Maximum number of distinct streams (grouped by TenantId + StreamType + StreamId)
    /// to archive in a single job run. Prevents long-running jobs.
    /// Default: 100.
    /// </summary>
    public int BatchSize { get; set; } = 100;
}
