using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.UI.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Manages application lifecycle events to coordinate initialization, UI readiness, and full application loading.
/// This service eliminates race conditions by providing a clear, event-driven state machine for application startup.
/// </summary>
public sealed class ApplicationLifecycleService : IApplicationLifecycleService
{
    private readonly ILogger<ApplicationLifecycleService> _logger;
    private volatile int _applicationUIReadyPublished = 0; // 0 = not sent, 1 = sent
    private volatile int _applicationInitializedPublished = 0; // 0 = not sent, 1 = sent
    private volatile int _applicationLoadedPublished = 0; // 0 = not sent, 1 = sent

    /// <summary>
    /// Raised when the UI framework is ready (window created, first page displayed).
    /// This is the earliest point where dialogs and modal navigation are safe.
    /// </summary>
    public event EventHandler<EventArgs>? ApplicationUIReady;

    /// <summary>
    /// Raised when application initialization is complete (backend services initialized, data loaded).
    /// This indicates business logic initialization is done.
    /// </summary>
    public event EventHandler<EventArgs>? ApplicationInitialized;

    /// <summary>
    /// Raised when both UI is ready AND application is initialized.
    /// At this point, the app is fully operational and ready for user interaction.
    /// This replaces the legacy ApplicationLoadedMessage.
    /// </summary>
    public event EventHandler<EventArgs>? ApplicationLoaded;

    public ApplicationLifecycleService(ILogger<ApplicationLifecycleService> logger)
    {
        Argument.IsNotNull(logger);
        _logger = logger;
        _logger.LogTrace("ApplicationLifecycleService instantiated");
    }

    /// <summary>
    /// Signals that the UI framework is ready. Should be called from EmptyView.Loaded event.
    /// </summary>
    public void SignalApplicationUIReady()
    {
        if (Interlocked.CompareExchange(ref _applicationUIReadyPublished, 1, 0) == 0)
        {
            try
            {
                _logger.LogTrace("SignalUiReady: UI framework is now ready");
                ApplicationUIReady?.Invoke(this, EventArgs.Empty);
                TryRaiseApplicationLoaded();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UiReady event handlers");
            }
        }
        else
        {
            _logger.LogWarning("SignalUiReady called multiple times; ignoring subsequent calls");
        }
    }

    /// <summary>
    /// Signals that application initialization is complete. Should be called from InitializeApplicationAsync.
    /// </summary>
    public void SignalApplicationInitialized()
    {
        if (Interlocked.CompareExchange(ref _applicationInitializedPublished, 1, 0) == 0)
        {
            try
            {
                _logger.LogTrace("SignalApplicationInitialized: Application initialization is complete");
                ApplicationInitialized?.Invoke(this, EventArgs.Empty);
                TryRaiseApplicationLoaded();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ApplicationInitialized event handlers");
            }
        }
        else
        {
            _logger.LogWarning("SignalApplicationInitialized called multiple times; ignoring subsequent calls");
        }
    }

    /// <summary>
    /// Signals that the application is fully loaded and ready for use.
    /// Should be called when both UI and initialization are complete.
    /// </summary>
    public void SignalApplicationLoaded()
    {
        SignalApplicationInitialized();
    }

    /// <summary>
    /// Attempts to raise ApplicationLoaded if both UI and initialization are ready.
    /// Uses atomic operation to ensure it's only raised once.
    /// </summary>
    private void TryRaiseApplicationLoaded()
    {
        // Only proceed if both signals have been received
        if (_applicationUIReadyPublished == 1 && _applicationInitializedPublished == 1)
        {
            // Use CompareExchange to atomically check and set the published flag
            if (Interlocked.CompareExchange(ref _applicationLoadedPublished, 1, 0) == 0)
            {
                try
                {
                    _logger.LogTrace("ApplicationLoaded: Both UI and initialization complete, raising ApplicationLoaded");
                    ApplicationLoaded?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ApplicationLoaded event handlers");
                }
            }
        }
    }

    /// <summary>
    /// Determines if the UI is ready.
    /// </summary>
    public bool IsApplicationUIReady => _applicationUIReadyPublished == 1;

    /// <summary>
    /// Determines if application initialization is complete.
    /// </summary>
    public bool IsApplicationInitialized => _applicationInitializedPublished == 1;

    /// <summary>
    /// Determines if the application is fully loaded and ready.
    /// </summary>
    public bool IsApplicationLoaded => _applicationLoadedPublished == 1;

    public void Dispose()
    {
        _logger.LogTrace("ApplicationLifecycleService disposing");

        // Unsubscribe all event handlers to prevent memory leaks
        ApplicationUIReady = null;
        ApplicationInitialized = null;
        ApplicationLoaded = null;
    }
}
