using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Logging.Extensions;

/// <summary>
/// Extension methods for working with telemetry.
/// </summary>
public static class TelemetryExtensions
{
    /// <summary>
    /// Sets common exception attributes on an Activity.
    /// </summary>
    public static void SetExceptionTags(this Activity activity, Exception exception)
    {
        if (activity == null)
            return;

        activity.SetTag("error", true);
        activity.SetTag("error.type", exception.GetType().Name);
        activity.SetTag("error.message", exception.Message);
        activity.SetTag("error.stack", exception.StackTrace);
    }

    /// <summary>
    /// Creates an activity that automatically starts and ends around a method call.
    /// </summary>
    public static async Task WithActivityAsync(
        this ActivitySource activitySource,
        Func<Task> action,
        [CallerMemberName] string? memberName = null)
    {
        using var activity = activitySource.StartActivity($"{memberName}");
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            activity?.SetExceptionTags(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates an activity that automatically starts and ends around a method call with a return value.
    /// </summary>
    public static async Task<T> WithActivityAsync<T>(
        this ActivitySource activitySource,
        Func<Task<T>> action,
        [CallerMemberName] string? memberName = null)
    {
        using var activity = activitySource.StartActivity($"{memberName}");
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            activity?.SetExceptionTags(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates an activity that automatically starts and ends around a synchronous method call.
    /// </summary>
    public static void WithActivity(
        this ActivitySource activitySource,
        Action action,
        [CallerMemberName] string? memberName = null)
    {
        using var activity = activitySource.StartActivity($"{memberName}");
        try
        {
            action();
        }
        catch (Exception ex)
        {
            activity?.SetExceptionTags(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates an activity that automatically starts and ends around a synchronous method call with a return value.
    /// </summary>
    public static T WithActivity<T>(
        this ActivitySource activitySource,
        Func<T> action,
        [CallerMemberName] string? memberName = null)
    {
        using var activity = activitySource.StartActivity($"{memberName}");
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            activity?.SetExceptionTags(ex);
            throw;
        }
    }
}
