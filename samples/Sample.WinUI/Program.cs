using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Extensions;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Hosting;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Abstractions.Services;
using Sample.OpenTelemetry;
using Sample.Services;
using System.Diagnostics;
using System.Reflection;

namespace Sample;

public static class Program
{
    [STAThread] // Required for WinUI apps
    public static Task Main(string[] args)
    {
        var builder = new WindowsAppSdkHostBuilder<App>();
        var mainAssembly = Assembly.GetAssembly(typeof(App)) ?? throw new InvalidOperationException("Unable to load main assembly");
        var infoService = new InfoService();
        infoService.LoadAssembly(mainAssembly);

        var resourceStream = mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json")
            ?? throw new InvalidOperationException("Unable to load appsettings resource");
        builder.Configuration.AddJsonStream(resourceStream);

        builder.Logging
            .AddTelemetry(
                builder,
                infoService,
                tracerProviderBuilderAction: (tracing) =>
                {
                    // Additional custom tracer provider configuration can be added here
                },
                meterProviderBuilderAction: (metrics) =>
                {
                    // Additional custom meter provider configuration can be added here
                },
                loggerProviderBuilderAction: (logger) =>
                {
                    // Additional custom logger provider configuration can be added here
                })
            .AddOtlpExporter();

        builder
            .ConfigureServices<App, Context, CommonServices, ExceptionHandlerService, SettingsService, Properties.Resources>(appBuilder =>
            {
                appBuilder.Services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                appBuilder.Services.TryAddSingleton<ICameraService, CameraService>();
                appBuilder.Services.TryAddSingleton<ApplicationMonitor>();
            },
                mainAssembly,
                f => f.Name!.StartsWith(typeof(App).Namespace!))
            .ConfigureSupportedCultures(cultures =>
            {
                cultures.AddCulture("nl");
                cultures.AddCulture("de");
                cultures.AddCulture("fr");
                cultures.AddCulture("ar");
            })
            //.UseThemeColors(colors => colors.Add("#0078D4"))
            .ConfigureSingleInstance()
            .ConfigureActivationHandlers(e =>
            {
                foreach (var arg in e.EnsureNotNull())
                {
                    if (!string.IsNullOrEmpty(arg))
                        Debug.WriteLine(string.Format("Environment: {0}", arg));
                }
            }, async p =>
            {
                if (!string.IsNullOrEmpty(p.Uri?.Query))
                    Debug.WriteLine(string.Format("Protocol: {0}", p.Uri.Query));
            }, async l =>
            {
                if (!string.IsNullOrEmpty(l.Arguments))
                    Debug.WriteLine(string.Format("Launch: {0}", l.Arguments));
            });
        //.ConfigureOfflineSynchronization(builder =>
        //{
        //    Debug.WriteLine("Configuring offline synchronization");
        //});

        var app = builder.Build();

        return app.StartAsync();
    }
}