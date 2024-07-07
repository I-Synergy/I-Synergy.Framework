using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Models;
using Sample.Services;
using System.Reflection;

namespace Sample;

public static class Program
{
    [STAThread]
    public static Task Main(string[] args)
    {
        var builder = new WindowsAppSdkHostBuilder<App>();
        var mainAssembly = Assembly.GetAssembly(typeof(App));

        builder
            .ConfigureAppConfiguration(configBuilder =>
            {
                var config = new ConfigurationBuilder();
                configBuilder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
                configBuilder.AddConfiguration(config.Build());
            })
            .ConfigureLogging<App>((logging, configuration) =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources>(services =>
            {
                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

                services.TryAddSingleton<IApplicationSettingsService, LocalSettingsService>();
                services.TryAddSingleton<ISettingsService<GlobalSettings>, GlobalSettingsService>();

                services.TryAddSingleton<CommonServices>();
                services.TryAddSingleton<IBaseCommonServices>(s => s.GetRequiredService<CommonServices>());
                services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<CommonServices>());
            });
            //.ConfigureStoreUpdateIntegration()
            //.ConfigureSingleInstanceApp(async e =>
            //{
            //    foreach (var arg in e.EnsureNotNull())
            //    {
            //        if (!string.IsNullOrEmpty(arg))
            //            await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync(arg, "Environment");
            //    }
            //}, async p =>
            //{
            //    if (!string.IsNullOrEmpty(p.Uri?.Query))
            //        await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync($"{p.Uri.Query}", "ProtocolActivatedEventArgs");
            //}, async l =>
            //{
            //    if (!string.IsNullOrEmpty(l.Arguments))
            //        await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync($"{l.Arguments}", "LaunchActivatedEventArgs");
            //})
            //.ConfigureOfflineSynchronization(builder =>
            //{
            //    Debug.WriteLine("Configuring offline synchronization");
            //    Debug.WriteLine(localSettingsService.Settings.IsSynchronizationEnabled);
            //})
            //.ConfigureSyncfusionCore();

        var app = builder.Build();

        return app.StartAsync();
    }
}
