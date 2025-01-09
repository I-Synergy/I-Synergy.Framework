using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NugetUnlister.Extensions;
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
            .ConfigureServices<App, Context, CommonServices, AuthenticationService, SettingsService<LocalSettings, RoamingSettings, GlobalSettings>, Properties.Resources>((services, configuration) =>
            {
                services.TryAddSingleton<IUnitConversionService, UnitConversionService>();

                services.AddNugetServiceIntegrations(configuration);
            })
            .Build();

        App application = new();
        application.InitializeComponent();
        application.Run();
    }
}
