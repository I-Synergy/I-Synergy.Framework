using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Globalization.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using WinRT;
using HostOptions = ISynergy.Framework.UI.Options.HostOptions;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

namespace ISynergy.Framework.UI.Hosting;

internal sealed class WindowsAppSdkHost<TApp> : IHost, IAsyncDisposable
    where TApp : Application, new()
{
    private readonly ILogger<WindowsAppSdkHost<TApp>> _logger;
    private readonly IHostLifetime _hostLifetime;
    private readonly ApplicationLifetime _applicationLifetime;
    private readonly HostOptions _options;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly PhysicalFileProvider _defaultProvider;
    private IEnumerable<IHostedService>? _hostedServices;
    private volatile bool _stopCalled;

    public WindowsAppSdkHost(
        IServiceProvider services,
        IHostEnvironment hostEnvironment,
        PhysicalFileProvider defaultProvider,
        IHostApplicationLifetime applicationLifetime,
        ILogger<WindowsAppSdkHost<TApp>> logger,
        IHostLifetime hostLifetime,
        IOptions<HostOptions> options)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));

        _applicationLifetime = (applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime))) as ApplicationLifetime;
        _hostEnvironment = hostEnvironment;
        _defaultProvider = defaultProvider;

        if (_applicationLifetime is null)
        {
            throw new ArgumentException(LanguageService.Default.GetString("WindowsAppSdkHostReplacingIHostApplicationLifetimeIsNotSupported"), nameof(applicationLifetime));
        }
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hostLifetime = hostLifetime ?? throw new ArgumentNullException(nameof(hostLifetime));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public IServiceProvider Services
    {
        get;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        //        _logger.Starting();

        using var combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _applicationLifetime.ApplicationStopping);
        var combinedCancellationToken = combinedCancellationTokenSource.Token;

        await _hostLifetime.WaitForStartAsync(combinedCancellationToken).ConfigureAwait(false);

        combinedCancellationToken.ThrowIfCancellationRequested();
        _hostedServices = Services.GetService<IEnumerable<IHostedService>>()!;

        foreach (var hostedService in _hostedServices.EnsureNotNull())
        {
            // Fire IHostedService.Start
            await hostedService.StartAsync(combinedCancellationToken).ConfigureAwait(false);

            if (hostedService is BackgroundService backgroundService)
            {
                _ = TryExecuteBackgroundServiceAsync(backgroundService);
            }
        }

        HostOptions.XamlCheckProcessRequirements();

        ComWrappersSupport.InitializeComWrappers();

        // ── Single-instance enforcement (opt-in) ────────────────
        // Only enforced when ConfigureSingleInstance() was called on the builder.
        if (AppActivationExtensions.IsSingleInstanceEnabled)
        {
            // Check for existing instances BEFORE creating the Application.
            // No SynchronizationContext exists yet, so .Wait() is safe.
            var instances = AppInstance.GetInstances();
            Debug.WriteLine($"[SingleInstance] GetInstances returned {instances.Count} instance(s).");

            AppInstance? existing = null;
            foreach (var instance in instances)
            {
                if (!instance.IsCurrent)
                {
                    existing = instance;
                    break;
                }
            }

            if (existing is not null)
            {
                // Second instance — redirect activation and exit
                Debug.WriteLine("[SingleInstance] Existing instance found. Redirecting...");
                var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                Debug.WriteLine($"[SingleInstance] Activation kind: {activationArgs.Kind}");

                try
                {
                    existing.RedirectActivationToAsync(activationArgs).AsTask().Wait();
                    Debug.WriteLine("[SingleInstance] Redirect completed.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SingleInstance] Redirect failed: {ex}");
                }

                Debug.WriteLine("[SingleInstance] Exiting second instance.");
                return;
            }

            // First instance — subscribe to future redirected activations
            Debug.WriteLine("[SingleInstance] First instance. Subscribing to Activated event.");
            AppInstance.GetCurrent().Activated += OnAppInstanceActivated;
            Debug.WriteLine("[SingleInstance] Ready to receive redirected activations.");
        }
        // ── End single-instance enforcement ─────────────────────

        // Make the final service provider available via ServiceLocator before TApp is constructed,
        // so the Application base constructor can resolve services before Services is injected.
        ServiceLocator.SetLocatorProvider(Services);

        async void OnAppOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            _logger.LogCritical(args.Exception, "Unhandled exception.  Terminating.");
            args.Handled = false;
            await RequestExit(combinedCancellationTokenSource);
        }

        Application.Start(
            _ =>
            {
                try
                {
                    var context = new DispatcherQueueSynchronizationContext(
                        DispatcherQueue.GetForCurrentThread()
                    );
                    SynchronizationContext.SetSynchronizationContext(context);
                    var app = Services.GetRequiredService<TApp>();

                    app.UnhandledException += OnAppOnUnhandledException;

                    if (app is Application ca)
                    {
                        ca.Services = Services;
                        ca.Token = combinedCancellationToken;
                    }

                    // Fire IHostApplicationLifetime.Started
                    _applicationLifetime.NotifyStarted();

                    // Process initial activation args (command-line, protocol URI, etc.)
                    // from the first launch. Defer with low priority so the UI window
                    // has been created before callbacks try to show dialogs.
                    if (AppActivationExtensions.IsSingleInstanceEnabled)
                    {
                        var initialArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                        Debug.WriteLine($"[SingleInstance] Processing initial activation: Kind={initialArgs.Kind}");
                        DispatcherQueue.GetForCurrentThread().TryEnqueue(
                            DispatcherQueuePriority.Low,
                            () => AppActivationExtensions.Dispatch(null, initialArgs));
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while executing (in lambda).");
                }
            }
        );

        try
        {
            _logger.LogInformation("Application is exiting.");
            await RequestExit(combinedCancellationTokenSource);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while exiting.");
        }
    }

    private async Task RequestExit(CancellationTokenSource source)
    {
        var token = source.Token;
        token.ThrowIfCancellationRequested();
        source.CancelAfter(TimeSpan.FromMinutes(1));

        try
        {
            await StopAsync(token);
        }
        finally
        {
            await DisposeAsync();
        }
    }

    private async Task TryExecuteBackgroundServiceAsync(BackgroundService backgroundService)
    {
        try
        {
            if (backgroundService.ExecuteTask is not null)
                await backgroundService.ExecuteTask;
        }
        catch (Exception ex)
        {
            // When the host is being stopped, it cancels the background services.
            // This isn't an error condition, so don't log it as an error.
            if (_stopCalled && backgroundService.ExecuteTask is not null && backgroundService.ExecuteTask.IsCanceled && ex is OperationCanceledException)
            {
                return;
            }

            //_logger.BackgroundServiceFaulted(ex);
            if (_options.BackgroundServiceExceptionBehavior == BackgroundServiceExceptionBehavior.StopHost)
            {
                //_logger.BackgroundServiceStoppingHost(ex);
                _applicationLifetime.StopApplication();
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _stopCalled = true;
        //_logger.Stopping();

        using var cts = new CancellationTokenSource(_options.ShutdownTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

        var token = linkedCts.Token;
        // Trigger IHostApplicationLifetime.ApplicationStopping
        _applicationLifetime.StopApplication();

        IList<Exception> exceptions = new List<Exception>();
        if (_hostedServices != null) // Started?
        {
            foreach (var hostedService in _hostedServices.Reverse().EnsureNotNull())
            {
                try
                {
                    await hostedService.StopAsync(token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        }

        // Fire IHostApplicationLifetime.Stopped
        _applicationLifetime.NotifyStopped();

        try
        {
            Application.Current?.Exit();
            await _hostLifetime.StopAsync(token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exceptions.Add(ex);
        }

        if (exceptions.Count > 0)
        {
            var ex = new AggregateException("One or more hosted services failed to stop.", exceptions);
            //_logger.StoppedWithException(ex);
            throw ex;
        }

        //_logger.Stopped();
    }

    /// <summary>
    /// Handles redirected activations from second instances.
    /// Dispatches to the registered activation callbacks.
    /// </summary>
    private static void OnAppInstanceActivated(object? sender, AppActivationArguments e)
    {
        Debug.WriteLine($"[SingleInstance] Activated event fired: Kind={e.Kind}");
        AppActivationExtensions.Dispatch(sender, e);
    }

    public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();

    public async ValueTask DisposeAsync()
    {
        // The user didn't change the ContentRootFileProvider instance, we can dispose it
        if (ReferenceEquals(_hostEnvironment.ContentRootFileProvider, _defaultProvider))
        {
            // Dispose the content provider
            await DisposeAsyncLocal(_hostEnvironment.ContentRootFileProvider).ConfigureAwait(false);
        }
        else
        {
            // In the rare case that the user replaced the ContentRootFileProvider, dispose it and the one
            // we originally created
            await DisposeAsyncLocal(_hostEnvironment.ContentRootFileProvider).ConfigureAwait(false);
            await DisposeAsyncLocal(_defaultProvider).ConfigureAwait(false);
        }

        // Dispose the application.
        await DisposeAsyncLocal(Application.Current).ConfigureAwait(false);

        // Dispose the service provider
        await DisposeAsyncLocal(Services).ConfigureAwait(false);

        static async ValueTask DisposeAsyncLocal(object o)
        {
            switch (o)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
}

