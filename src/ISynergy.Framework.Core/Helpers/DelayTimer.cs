using ITimer = ISynergy.Framework.Core.Abstractions.Timers.ITimer;

namespace ISynergy.Framework.Core.Helpers;

/// <summary>
/// A timer based on Task.Delay.
/// </summary>
public class DelayTimer : ITimer
{
    /// <summary>
    /// Start the loop with specified interval and step action. The loop stops when
    /// the step returns false or cancellation is requested.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="step">The step.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to stop the timer.</param>
    /// <returns>A task that completes when the timer stops.</returns>
    public async Task StartAsync(TimeSpan interval, Func<bool> step, CancellationToken cancellationToken = default)
    {
        try
        {
            var shouldContinue = step();

            while (!cancellationToken.IsCancellationRequested && shouldContinue)
            {
                await Task.Delay(interval, cancellationToken);
                shouldContinue = step();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"DelayTimer exception: {ex}");
        }
    }
}
