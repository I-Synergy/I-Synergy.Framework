using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Hosting;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Sample.Abstractions.Services;
using Sample.Services;
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

        builder
            .ConfigureLogging(
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
            .ConfigureServices<App, Context, CommonServices, ExceptionHandlerService, SettingsService, Properties.Resources>(services =>
            {
                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<ICameraService, CameraService>();
            },
                mainAssembly,
                f => f.Name!.StartsWith(typeof(App).Namespace!))
            .ConfigureSingleInstance()
            .ConfigureActivationHandlers(async e =>
            {
                foreach (var arg in e.EnsureNotNull())
                {
                    if (!string.IsNullOrEmpty(arg))
                        await ServiceLocator.Default.GetRequiredService<IDialogService>().ShowMessageAsync(arg, "Environment");
                }
            }, async p =>
            {
                if (!string.IsNullOrEmpty(p.Uri?.Query))
                    await ServiceLocator.Default.GetRequiredService<IDialogService>().ShowMessageAsync($"{p.Uri.Query}", "ProtocolActivatedEventArgs");
            }, async l =>
            {
                if (!string.IsNullOrEmpty(l.Arguments))
                    await ServiceLocator.Default.GetRequiredService<IDialogService>().ShowMessageAsync($"{l.Arguments}", "LaunchActivatedEventArgs");
            });
        //.ConfigureOfflineSynchronization(builder =>
        //{
        //    Debug.WriteLine("Configuring offline synchronization");
        //});

        var app = builder.Build();

        return app.StartAsync();
    }
}