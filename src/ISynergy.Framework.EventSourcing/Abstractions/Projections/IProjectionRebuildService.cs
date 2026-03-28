namespace ISynergy.Framework.EventSourcing.Abstractions.Projections;

/// <summary>
/// Rebuilds a read-model projection by replaying the full event stream for a given aggregate type.
/// </summary>
public interface IProjectionRebuildService
{
    /// <summary>
    /// Replays all events for <paramref name="aggregateType"/> and rebuilds the projection from scratch.
    /// </summary>
    Task RebuildAsync(string aggregateType, CancellationToken cancellationToken = default);
}
