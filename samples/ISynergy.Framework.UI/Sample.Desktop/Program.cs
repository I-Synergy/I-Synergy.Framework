using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Logging.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Views;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Hosting;
using ISynergy.Framework.Update.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Sample.Abstractions.Services;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using System.Reflection;

namespace Sample
{
    public static class Program
    {
        [STAThread]
        public static async Task Main(string[] args)
        {
            var host = new WindowsAppSdkHostBuilder<App>()
                .ConfigureHostConfiguration(builder =>
                {
                    var mainAssembly = Assembly.GetAssembly(typeof(App));
                    builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
                })
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureServices<App, Context, Sample.Properties.Resources>(context.Configuration, x => x.Name.StartsWith(typeof(App).Namespace));

                    services.AddSingleton<IAuthenticationService, AuthenticationService>();

                    services.AddUpdatesIntegration();

                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, AppSettingsService>());
                    services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService<Setting>, SettingsService>());

                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
                    services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

                    services.AddTransient<IAuthenticationView, AuthenticationView>();
                    services.AddTransient<IShellViewModel, ShellViewModel>();
                    services.AddTransient<IShellView, ShellView>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    //logging.AddAppCenterLogging(context.Configuration);
                    logging.AddSentryLogging(context.Configuration);
                })
                .Build();

            await host.StartAsync();
        }
    }
}
