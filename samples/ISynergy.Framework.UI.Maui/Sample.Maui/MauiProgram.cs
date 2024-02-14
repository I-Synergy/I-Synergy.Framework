using CommunityToolkit.Maui;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Locators;
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

namespace Sample;

public class MauiProgram
{
    private static readonly object _creationLock = new object();
    private static MauiProgram _defaultInstance;

    public static MauiProgram Default
    {
        get
        {
            if (_defaultInstance is null)
            {
                lock (_creationLock)
                {
                    if (_defaultInstance is null)
                        _defaultInstance = new MauiProgram();
                }
            }

            return _defaultInstance;
        }
    }

    public async Task<MauiApp> CreateMauiAppAsync()
    {
        var builder = MauiApp.CreateBuilder();

        Assembly mainAssembly = Assembly.GetAssembly(typeof(App));

        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonStream(await FileSystem.OpenAppPackageFileAsync("appsettings.json"))
            .Build();

        builder
            .Configuration
            .AddConfiguration(config);

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Font Awesome 6 Pro-Regular-400.otf", "fontawesome");
            })
            .ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources>(x => x.Name.StartsWith(typeof(MauiProgram).Namespace));

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Logging.SetMinimumLevel(LogLevel.Trace);

        builder.Services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

        builder.Services.TryAddSingleton<IBaseApplicationSettingsService, LocalSettingsService>();
        builder.Services.TryAddSingleton<ISettingsService<GlobalSettings>, GlobalSettingsService>();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

        ServiceLocator.SetLocatorProvider(builder.Services.BuildServiceProvider());

        return builder.Build();
    }

    
}
