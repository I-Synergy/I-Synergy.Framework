using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Logging.Extensions;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
        var mainAssembly = Argument.IsNotNull(Assembly.GetAssembly(typeof(App)));
        var infoService = new InfoService();
        infoService.LoadAssembly(mainAssembly);

        IHost host = new HostBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json")!);
            })
            .ConfigureLogging((logging, configuration) =>
            {
                logging.AddOpenTelemetryLogging(configuration);
            })
            .ConfigureServices<App, Context, CommonServices, AuthenticationService, SettingsService<LocalSettings, RoamingSettings, GlobalSettings>, Properties.Resources>((services, configuration) =>
            {
                services.TryAddSingleton<IUnitConversionService, UnitConversionService>();

                services.AddNugetServiceIntegrations(configuration);
            })
            .ConfigureOpenTelemetryLogging(infoService)
            .Build();

        App application = new();
        application.InitializeComponent();
        application.Run();
    }
}
