using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Adapters;
using ISynergy.Framework.OpenTelemetry.Initializers;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace ISynergy.Framework.OpenTelemetry.Providers;
/// <summary>
/// Application Insights implementation of ITelemetryProvider.
/// </summary>
public class ApplicationInsightsProvider : OpenTelemetryProvider
{
    private TelemetryClient? _client;

    /// <summary>
    /// Configures Application Insights with the specified service collection and options.
    /// </summary>
    public override void Configure(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        IInfoService infoService,
        string defaultTelemetrySection,
        Action<TracerProviderBuilder>? tracingAction = null,
        Action<MeterProviderBuilder>? metricsAction = null,
        Action<LoggerProviderBuilder>? loggerAction = null)
    {
        base.Configure(services, configuration, environment, infoService, defaultTelemetrySection, tracingAction, metricsAction, loggerAction);

        services.TryAddScoped<ITelemetryInitializer, DefaultTelemetryInitializer>();

        var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        configuration.GetSection(defaultTelemetrySection).Bind(telemetryConfiguration);

        //telemetryConfiguration.TelemetryInitializers.Add(scopedContextService.GetService<ITelemetryInitializer>());

        var temporaryLogFolder = Path.Combine(
            Path.GetTempPath(),
            Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName),
            "Logging");

        if (!Directory.Exists(temporaryLogFolder))
            Directory.CreateDirectory(temporaryLogFolder);

        var serverTelemetryChannel = new ServerTelemetryChannel();
        serverTelemetryChannel.StorageFolder = temporaryLogFolder;

#if DEBUG
        serverTelemetryChannel.DeveloperMode = Debugger.IsAttached;
#endif
        serverTelemetryChannel.Initialize(telemetryConfiguration);
        telemetryConfiguration.TelemetryChannel = serverTelemetryChannel;

        _client = new TelemetryClient(telemetryConfiguration);
        _client.Context.User.UserAgent = infoService.ProductName;
        _client.Context.Component.Version = infoService.ProductVersion.ToString();
        _client.Context.Session.Id = Guid.NewGuid().ToString();
        _client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Flush();

        // Register the TelemetryClientAdapter to bridge between OpenTelemetry and Application Insights
        services.AddSingleton(provider =>
        {
            return new TelemetryClientAdapter(_client, _activitySource!);
        });
    }

    /// <summary>
    /// Flushes this instance.
    /// </summary>
    public void Flush()
    {
        if (_client != null)
            _client.Flush();

        // Explicitly call Flush() followed by Delay, as required in console apps.
        // This ensures that even if the application terminates, telemetry is sent to the back end.
        System.Threading.Thread.Sleep(5000);
    }
}