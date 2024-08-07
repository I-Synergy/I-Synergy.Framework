using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
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
    private IEnumerable<IHostedService> _hostedServices;
    private volatile bool _stopCalled;

    public WindowsAppSdkHost(IServiceProvider services,
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
            throw new ArgumentException(ISynergy.Framework.UI.Properties.Resources.WindowsAppSdkHostReplacingIHostApplicationLifetimeIsNotSupported, nameof(applicationLifetime));
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

#if WINDOWS
        ComWrappersSupport.InitializeComWrappers();
#endif

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
                    var app = Services.GetRequiredService<TApp>();

                    app.UnhandledException += OnAppOnUnhandledException;

                    if (app is CancelableApplication ca)
                    {
                        ca.Services = Services;
                        ca.Token = combinedCancellationToken;
                    }

                    // Fire IHostApplicationLifetime.Started
                    _applicationLifetime.NotifyStarted();
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
            await backgroundService.ExecuteTask.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // When the host is being stopped, it cancels the background services.
            // This isn't an error condition, so don't log it as an error.
            if (_stopCalled && backgroundService.ExecuteTask.IsCanceled && ex is OperationCanceledException)
            {
                return;
            }

            if (_options.BackgroundServiceExceptionBehavior == BackgroundServiceExceptionBehavior.StopHost)
            {
                _applicationLifetime.StopApplication();
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _stopCalled = true;

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
            throw ex;
        }
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
