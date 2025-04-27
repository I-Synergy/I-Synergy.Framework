using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.OpenTelemetry.Adapters;

/// <summary>
/// Adapter to bridge between OpenTelemetry Activity concepts and Application Insights.
/// </summary>
public class TelemetryClientAdapter
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ActivitySource _activitySource;

    public TelemetryClientAdapter(TelemetryClient telemetryClient, ActivitySource activitySource)
    {
        _telemetryClient = telemetryClient;
        _activitySource = activitySource;
    }

    /// <summary>
    /// Gets the activity source.
    /// </summary>
    public ActivitySource ActivitySource => _activitySource;

    /// <summary>
    /// Creates a new activity that corresponds to an Application Insights operation.
    /// </summary>
    public Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        var activity = _activitySource.StartActivity(name, kind);

        if (activity != null)
        {
            // Create a corresponding Application Insights operation
            var operation = new RequestTelemetry
            {
                Name = name,
                Id = activity.Id,
                Context = { Operation = { Id = activity.TraceId.ToString() } }
            };

            // Store the operation ID in the activity's tags
            activity.SetTag("ai.operation.id", operation.Context.Operation.Id);
        }

        return activity;
    }

    /// <summary>
    /// Creates an activity for a method call with Application Insights integration.
    /// </summary>
    public async Task WithActivityAsync(Func<Task> action, [CallerMemberName] string? memberName = null)
    {
        using var activity = StartActivity(memberName ?? "UnnamedActivity");
        var startTime = DateTimeOffset.UtcNow;
        var timer = Stopwatch.StartNew();

        try
        {
            await action();

            // Track successful dependency
            if (activity != null)
            {
                _telemetryClient.TrackDependency(
                    dependencyTypeName: "InProc",
                    target: memberName ?? "UnnamedActivity",
                    dependencyName: memberName ?? "UnnamedActivity",
                    data: null,
                    startTime: startTime,
                    duration: timer.Elapsed,
                    resultCode: StatusCode.Ok.ToString(),
                    success: true);
            }
        }
        catch (Exception ex)
        {
            // Track exception and failed dependency
            _telemetryClient.TrackException(ex);

            if (activity != null)
            {
                _telemetryClient.TrackDependency(
                    dependencyTypeName: "InProc",
                    target: memberName ?? "UnnamedActivity",
                    dependencyName: memberName ?? "UnnamedActivity",
                    data: null,
                    startTime: startTime,
                    duration: timer.Elapsed,
                    resultCode: StatusCode.Error.ToString(),
                    success: false);
            }

            throw;
        }
    }

    /// <summary>
    /// Creates an activity for a method call with a return value with Application Insights integration.
    /// </summary>
    public async Task<T> WithActivityAsync<T>(Func<Task<T>> action, [CallerMemberName] string? memberName = null)
    {
        using var activity = StartActivity(memberName ?? "UnnamedActivity");
        var startTime = DateTimeOffset.UtcNow;
        var timer = Stopwatch.StartNew();

        try
        {
            var result = await action();

            // Track successful dependency
            if (activity != null)
            {
                _telemetryClient.TrackDependency(
                    dependencyTypeName: "InProc",
                    target: memberName ?? "UnnamedActivity",
                    dependencyName: memberName ?? "UnnamedActivity",
                    data: null,
                    startTime: startTime,
                    duration: timer.Elapsed,
                    resultCode: StatusCode.Ok.ToString(),
                    success: true);
            }

            return result;
        }
        catch (Exception ex)
        {
            // Track exception and failed dependency
            _telemetryClient.TrackException(ex);

            if (activity != null)
            {
                _telemetryClient.TrackDependency(
                    dependencyTypeName: "InProc",
                    target: memberName ?? "UnnamedActivity",
                    dependencyName: memberName ?? "UnnamedActivity",
                    data: null,
                    startTime: startTime,
                    duration: timer.Elapsed,
                    resultCode: StatusCode.Error.ToString(),
                    success: false);
            }

            throw;
        }
    }

    /// <summary>
    /// Creates an activity for a synchronous method call with Application Insights integration.
    /// </summary>
    public void WithActivity(Action action, [CallerMemberName] string? memberName = null)
    {
        using var activity = StartActivity(memberName ?? "UnnamedActivity");
        var startTime = DateTimeOffset.UtcNow;
        var timer = Stopwatch.StartNew();

        try
        {
            action();

            // Track successful dependency
            if (activity != null)
            {
                _telemetryClient.TrackDependency(
                    dependencyTypeName: "InProc",
                    target: memberName ?? "UnnamedActivity",
                    dependencyName: memberName ?? "UnnamedActivity",
                    data: null,
                    startTime: startTime,
                    duration: timer.Elapsed,
                    resultCode: StatusCode.Ok.ToString(),
                    success: true);
            }
        }
        catch (Exception ex)
        {
            // Track exception and failed dependency
            _telemetryClient.TrackException(ex);

            if (activity != null)
            {
                _telemetryClient.TrackDependency(
                    dependencyTypeName: "InProc",
                    target: memberName ?? "UnnamedActivity",
                    dependencyName: memberName ?? "UnnamedActivity",
                    data: null,
                    startTime: startTime,
                    duration: timer.Elapsed,
                    resultCode: StatusCode.Error.ToString(),
                    success: false);
            }

            throw;
        }
    }

    /// <summary>
    /// Creates an activity for a synchronous method call with a return value with Application Insights integration.
    /// </summary>
    public T WithActivity<T>(Func<T> action, [CallerMemberName] string? memberName = null)
    {
        using var activity = StartActivity(memberName ?? "UnnamedActivity");
        var startTime = DateTimeOffset.UtcNow;
        var timer = Stopwatch.StartNew();

        try
        {
            var result = action();

            // Track successful dependency
            if (activity != null)
            {
                _telemetryClient.TrackDependency(
                    dependencyTypeName: "InProc",
                    target: memberName ?? "UnnamedActivity",
                    dependencyName: memberName ?? "UnnamedActivity",
                    data: null,
                    startTime: startTime,
                    duration: timer.Elapsed,
                    resultCode: StatusCode.Ok.ToString(),
                    success: true);
            }

            return result;
        }
        catch (Exception ex)
        {
            // Track exception and failed dependency
            _telemetryClient.TrackException(ex);

            if (activity != null)
            {
                _telemetryClient.TrackDependency(
                    dependencyTypeName: "InProc",
                    target: memberName ?? "UnnamedActivity",
                    dependencyName: memberName ?? "UnnamedActivity",
                    data: null,
                    startTime: startTime,
                    duration: timer.Elapsed,
                    resultCode: StatusCode.Error.ToString(),
                    success: false);
            }

            throw;
        }
    }

    /// <summary>
    /// Tracks a custom event in Application Insights.
    /// </summary>
    public void TrackEvent(string eventName, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null)
    {
        _telemetryClient.TrackEvent(eventName, properties, metrics);
    }
}