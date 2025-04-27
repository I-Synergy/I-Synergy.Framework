using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.OpenTelemetry.Adapters;

/// <summary>
/// Adapter to map OpenTelemetry Activity concepts to Sentry concepts.
/// </summary>
public class ActivityAdapter
{
    private readonly IHub _hub;
    private readonly ActivitySource _activitySource;

    public ActivityAdapter(IHub hub, ActivitySource activitySource)
    {
        _hub = hub;
        _activitySource = activitySource;
    }

    /// <summary>
    /// Gets the activity source.
    /// </summary>
    public ActivitySource ActivitySource => _activitySource;

    /// <summary>
    /// Creates a new activity that corresponds to a Sentry transaction or span.
    /// </summary>
    public Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        var activity = _activitySource.StartActivity(name, kind);

        if (activity != null)
        {
            // Check if we need to start a transaction or a span
            var currentSpan = _hub.GetSpan();

            if (currentSpan == null)
            {
                // Start a new transaction if there's no current span
                var transaction = _hub.StartTransaction(
                    new TransactionContext(
                        name: name,
                        operation: kind.ToString()
                    )
                );

                // Store the transaction in activity's baggage
                activity.SetBaggage("sentry.transaction", transaction.SpanId.ToString());

                // Set trace ID for correlation
                activity.SetTag("sentry.trace_id", transaction.TraceId.ToString());
            }
            else
            {
                // Start a child span under the current transaction
                var span = currentSpan.StartChild(
                    operation: kind.ToString(),
                    description: name
                );

                // Store the span in activity's baggage
                activity.SetBaggage("sentry.span", span.SpanId.ToString());

                // Set span ID for correlation
                activity.SetTag("sentry.span_id", span.SpanId.ToString());
            }
        }

        return activity;
    }

    /// <summary>
    /// Creates an activity with a corresponding Sentry span for a method call.
    /// </summary>
    public async Task WithActivityAsync(
        Func<Task> action,
        [CallerMemberName] string? memberName = null)
    {
        using var activity = StartActivity(memberName ?? "UnnamedActivity");

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            // Get the current transaction or span from the activity's baggage
            var spanIdStr = activity?.GetBaggageItem("sentry.span");
            var transactionIdStr = activity?.GetBaggageItem("sentry.transaction");

            _hub.CaptureException(ex, s =>
            {
                if (!string.IsNullOrEmpty(spanIdStr) || !string.IsNullOrEmpty(transactionIdStr))
                    s.Span = _hub.GetSpan();
            });
            throw;
        }
        finally
        {
            if (activity != null)
            {
                // Finish the Sentry span or transaction when the activity completes
                var spanIdStr = activity.GetBaggageItem("sentry.span");
                var transactionIdStr = activity.GetBaggageItem("sentry.transaction");

                var currentSpan = _hub.GetSpan();
                if (currentSpan != null)
                {
                    // If this activity created a transaction, or it's the current span, finish it
                    if ((!string.IsNullOrEmpty(transactionIdStr) && currentSpan.SpanId.ToString() == transactionIdStr) ||
                        (!string.IsNullOrEmpty(spanIdStr) && currentSpan.SpanId.ToString() == spanIdStr))
                    {
                        currentSpan.Finish();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates an activity with a corresponding Sentry span for a method call with a return value.
    /// </summary>
    public async Task<T> WithActivityAsync<T>(
        Func<Task<T>> action,
        [CallerMemberName] string? memberName = null)
    {
        using var activity = StartActivity(memberName ?? "UnnamedActivity");

        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            // Get the current transaction or span from the activity's baggage
            var spanIdStr = activity?.GetBaggageItem("sentry.span");
            var transactionIdStr = activity?.GetBaggageItem("sentry.transaction");

            _hub.CaptureException(ex, s =>
            {
                if (!string.IsNullOrEmpty(spanIdStr) || !string.IsNullOrEmpty(transactionIdStr))
                    s.Span = _hub.GetSpan();
            });
            throw;
        }
        finally
        {
            if (activity != null)
            {
                // Finish the Sentry span or transaction when the activity completes
                var spanIdStr = activity.GetBaggageItem("sentry.span");
                var transactionIdStr = activity.GetBaggageItem("sentry.transaction");

                var currentSpan = _hub.GetSpan();
                if (currentSpan != null)
                {
                    // If this activity created a transaction, or it's the current span, finish it
                    if ((!string.IsNullOrEmpty(transactionIdStr) && currentSpan.SpanId.ToString() == transactionIdStr) ||
                        (!string.IsNullOrEmpty(spanIdStr) && currentSpan.SpanId.ToString() == spanIdStr))
                    {
                        currentSpan.Finish();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates an activity with a corresponding Sentry span for a synchronous method call.
    /// </summary>
    public void WithActivity(
        Action action,
        [CallerMemberName] string? memberName = null)
    {
        using var activity = StartActivity(memberName ?? "UnnamedActivity");

        try
        {
            action();
        }
        catch (Exception ex)
        {
            // Get the current transaction or span from the activity's baggage
            var spanIdStr = activity?.GetBaggageItem("sentry.span");
            var transactionIdStr = activity?.GetBaggageItem("sentry.transaction");

            _hub.CaptureException(ex, s =>
            {
                if (!string.IsNullOrEmpty(spanIdStr) || !string.IsNullOrEmpty(transactionIdStr))
                    s.Span = _hub.GetSpan();
            });
            throw;
        }
        finally
        {
            if (activity != null)
            {
                // Finish the Sentry span or transaction when the activity completes
                var spanIdStr = activity.GetBaggageItem("sentry.span");
                var transactionIdStr = activity.GetBaggageItem("sentry.transaction");

                var currentSpan = _hub.GetSpan();
                if (currentSpan != null)
                {
                    // If this activity created a transaction, or it's the current span, finish it
                    if ((!string.IsNullOrEmpty(transactionIdStr) && currentSpan.SpanId.ToString() == transactionIdStr) ||
                        (!string.IsNullOrEmpty(spanIdStr) && currentSpan.SpanId.ToString() == spanIdStr))
                    {
                        currentSpan.Finish();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates an activity with a corresponding Sentry span for a synchronous method call with a return value.
    /// </summary>
    public T WithActivity<T>(
        Func<T> action,
        [CallerMemberName] string? memberName = null)
    {
        using var activity = StartActivity(memberName ?? "UnnamedActivity");

        try
        {
            return action();
        }
        catch (Exception ex)
        {
            // Get the current transaction or span from the activity's baggage
            var spanIdStr = activity?.GetBaggageItem("sentry.span");
            var transactionIdStr = activity?.GetBaggageItem("sentry.transaction");

            _hub.CaptureException(ex, s =>
            {
                if (!string.IsNullOrEmpty(spanIdStr) || !string.IsNullOrEmpty(transactionIdStr))
                    s.Span = _hub.GetSpan();
            });
            throw;
        }
        finally
        {
            if (activity != null)
            {
                // Finish the Sentry span or transaction when the activity completes
                var spanIdStr = activity.GetBaggageItem("sentry.span");
                var transactionIdStr = activity.GetBaggageItem("sentry.transaction");

                var currentSpan = _hub.GetSpan();
                if (currentSpan != null)
                {
                    // If this activity created a transaction, or it's the current span, finish it
                    if ((!string.IsNullOrEmpty(transactionIdStr) && currentSpan.SpanId.ToString() == transactionIdStr) ||
                        (!string.IsNullOrEmpty(spanIdStr) && currentSpan.SpanId.ToString() == spanIdStr))
                    {
                        currentSpan.Finish();
                    }
                }
            }
        }
    }
}
