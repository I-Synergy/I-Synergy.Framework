using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NugetUnlister.Extensions;
using Sample.Abstractions;
using Sample.Models;
using Sample.Services;
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
            .ConfigureLogging((logging, configuration) =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources>((services, configuration) =>
            {
                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<IUnitConversionService, UnitConversionService>();
                services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IApplicationSettingsService, LocalSettingsService>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService<GlobalSettings>, GlobalSettingsService>());

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

                services.AddNugetServiceIntegrations(configuration);
            })
            .Build();

        App application = new();
        application.InitializeComponent();
        application.Run();
    }
}
