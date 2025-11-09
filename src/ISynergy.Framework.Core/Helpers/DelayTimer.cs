using ITimer = ISynergy.Framework.Core.Abstractions.Timers.ITimer;

namespace ISynergy.Framework.Core.Helpers;

/// <summary>
/// A timer based on Task.Delay.
/// </summary>
public class DelayTimer : ITimer
{
    /// <summary>
    /// Start the loop with specified interval and step action. The loop stops as
    /// soon that the step return false.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="step">The step.</param>
    public async void Start(TimeSpan interval, Func<bool> step)
    {
        try
        {
            var shouldContinue = step();

            while (shouldContinue)
            {
                await Task.Delay(interval);
                shouldContinue = step();
            }
        }
        catch (Exception ex)
        {
            // Log exception to prevent it from being swallowed in async void method
            System.Diagnostics.Debug.WriteLine($"DelayTimer exception: {ex}");
            // Re-throw to crash the application if unhandled exception handler is configured
            throw;
        }
    }
}
