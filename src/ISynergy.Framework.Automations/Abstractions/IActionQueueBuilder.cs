using System.Collections.Concurrent;

namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Interface for building action execution queues.
/// </summary>
public interface IActionQueueBuilder
{
    /// <summary>
    /// Builds a queue of tasks from automation actions.
    /// </summary>
    /// <param name="automation">The automation containing actions.</param>
    /// <param name="value">The value to pass to actions.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <returns>A blocking collection containing task functions.</returns>
    Task<BlockingCollection<Func<Task>>> BuildQueueAsync(Automation automation, object value, CancellationTokenSource cancellationTokenSource);
}

