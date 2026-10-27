namespace ISynergy.Framework.Core.Abstractions.Timers;

/// <summary>
/// A repeated timer.
/// </summary>
public interface ITimer
{
    /// <summary>
    /// Start the timer and triggers the step action every elapsed interval, until false is returned
    /// or cancellation is requested.
    /// </summary>
    /// <param name="interval">The interval between steps.</param>
    /// <param name="step">An action executed every step. It should return false when finished.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to stop the timer.</param>
    /// <returns>A task that completes when the timer stops.</returns>
    Task StartAsync(TimeSpan interval, Func<bool> step, CancellationToken cancellationToken = default);
}
