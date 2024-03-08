using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Logging.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Models;
using Sample.Services;
using System.Reflection;

#if WINDOWS
using ISynergy.Framework.Update.Extensions;
#endif

namespace Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var mainAssembly = Assembly.GetAssembly(typeof(App));

        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonStream(mainAssembly.GetManifestResourceStream("Sample.appsettings.json"));
        var config = configBuilder.Build();

        builder
            .Configuration
            .AddConfiguration(config);

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Font Awesome 6 Pro-Regular-400.otf", "fontawesome");
            })
            .ConfigureLogging((logging, configuration) =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddAppCenterLogging(configuration);
            })
            .ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources>((services, configuration) => 
            {
                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

                services.TryAddSingleton<LocalSettingsService>();
                services.TryAddSingleton<IBaseApplicationSettingsService>(s => s.GetRequiredService<LocalSettingsService>());

                services.TryAddSingleton<ISettingsService<GlobalSettings>, GlobalSettingsService>();

                services.TryAddSingleton<CommonServices>();
                services.TryAddSingleton<IBaseCommonServices>(s => s.GetRequiredService<CommonServices>());
                services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<CommonServices>());

#if WINDOWS
                services.AddUpdatesIntegration();
#endif
            });

        return builder.Build();
    }
}
