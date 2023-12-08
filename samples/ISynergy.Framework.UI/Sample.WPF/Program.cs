using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NugetUnlister;
using NugetUnlister.Extensions;
using Sample.Abstractions.Services;
using Sample.Models;
using Sample.Services;
using Sample.Abstractions;
using System.Reflection;

namespace Sample;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        IHost host = new HostBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                Assembly mainAssembly = Assembly.GetAssembly(typeof(App));
                builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
            })
            .ConfigureServices((context, services) =>
            {
                services.ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources>(context.Configuration, x =>
                    x.Name.StartsWith(typeof(App).Namespace) ||
                    x.FullName.Equals(typeof(Identifier).Assembly.FullName));

                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<IUnitConversionService, UnitConversionService>();
                services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, AppSettingsService>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService<Setting>, SettingsService>());

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

                services.AddNugetServiceIntegrations(context.Configuration);
            })
            .ConfigureLogging((context, logging) =>
            {
            })
            .Build();

        using IServiceScope scope = host.Services.CreateScope();
        ServiceLocator.SetLocatorProvider(scope.ServiceProvider);

        App application = new();
        application.InitializeComponent();
        application.InitializeApplication();
        application.Run();
    }
}
