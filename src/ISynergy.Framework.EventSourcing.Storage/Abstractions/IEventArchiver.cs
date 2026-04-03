namespace ISynergy.Framework.EventSourcing.Storage.Abstractions;

/// <summary>
/// Runs the tiered storage archive job: takes snapshots, moves old events to cold storage,
/// and records metadata in the <c>EventArchiveIndex</c> table.
/// </summary>
public interface IEventArchiver
{
    /// <summary>
    /// Archives events older than the configured retention window.
    /// For each eligible stream: ensures a snapshot exists at or above the cutoff version,
    /// uploads the events to cold storage, records an index entry, and deletes the
    /// hot-tier rows from PostgreSQL.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A summary of what was archived.</returns>
    Task<EventArchiveResult> ArchiveOldEventsAsync(CancellationToken ct = default);
}

/// <summary>Summary returned by <see cref="IEventArchiver.ArchiveOldEventsAsync"/>.</summary>
/// <param name="StreamsArchived">Number of distinct streams that had events archived.</param>
/// <param name="EventsArchived">Total number of event rows moved to cold storage.</param>
/// <param name="Errors">Number of streams that failed to archive (logged; others continue).</param>
public record EventArchiveResult(int StreamsArchived, long EventsArchived, int Errors);
