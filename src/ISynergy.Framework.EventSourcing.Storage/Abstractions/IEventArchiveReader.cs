using ISynergy.Framework.EventSourcing.Models;

namespace ISynergy.Framework.EventSourcing.Storage.Abstractions;

/// <summary>
/// Reads the full event history for an aggregate by combining cold-tier storage archives
/// with the current hot-tier events in PostgreSQL.
/// </summary>
/// <remarks>
/// This is an audit/compliance read path. The normal aggregate load path
/// (via <c>IAggregateRepository</c>) uses snapshots and never touches cold storage.
/// </remarks>
public interface IEventArchiveReader
{
    /// <summary>
    /// Returns the complete ordered event history for the given aggregate,
    /// merging archived cold storage segments with current hot-tier events.
    /// </summary>
    /// <param name="tenantId">The owning tenant.</param>
    /// <param name="streamType">The aggregate type name (e.g. "Order").</param>
    /// <param name="aggregateId">The aggregate identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// All events ordered by <c>AggregateVersion</c> ascending, deduplicated.
    /// </returns>
    Task<IReadOnlyList<EventRecord>> GetFullHistoryAsync(
        Guid tenantId,
        string streamType,
        Guid aggregateId,
        CancellationToken ct = default);
}
