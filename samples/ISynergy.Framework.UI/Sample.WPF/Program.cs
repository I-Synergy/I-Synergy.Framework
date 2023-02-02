using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Sample.Abstractions.Services;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using System.Diagnostics;
using System.Reflection;

namespace Sample
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    var mainAssembly = Assembly.GetAssembly(typeof(App));
                    builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
                })
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureServices<App, Context, Sample.Properties.Resources>(context.Configuration, x => x.Name.StartsWith(typeof(App).Namespace));

                    services.AddSingleton<IAuthenticationService, AuthenticationService>();

                    //services.AddUpdatesIntegration(context.Configuration);

                    services.AddSingleton<IUnitConversionService, UnitConversionService>();

                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, AppSettingsService>());
                    services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService<Setting>, SettingsService>());

                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
                    services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

                    services.AddScoped<IShellViewModel, ShellViewModel>();
                    services.AddScoped<IShellView, ShellView>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    //logging.AddAppCenterLogging(context.Configuration);
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                ServiceLocator.SetLocatorProvider(scope.ServiceProvider);

                var application = new App();
                application.InitializeComponent();
                application.InitializeApplication();
                application.Run();
            }
        }
    }
}
