using CommunityToolkit.Maui;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Models;
using Sample.Services;
using Sample.Abstractions;
using System.Reflection;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Sample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            Assembly mainAssembly = Assembly.GetAssembly(typeof(App));

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"))
                .Build();

            builder.Configuration.AddConfiguration(config);

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("SegMDL2.ttf", "SegoeMdl2");
                    fonts.AddFont("opensans-regular.ttf", "opensans-regular");
                    fonts.AddFont("opensans-semibold.ttf", "opensans-semibold");
                    fonts.AddFont("opensans-medium.ttf", "sans-serif-medium");
                    fonts.AddFont("fontawesome.otf", "fontawesome");

                })
                .ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources>(x => x.Name.StartsWith(typeof(MauiProgram).Namespace));

            builder.Services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

            builder.Services.TryAddSingleton<IBaseApplicationSettingsService, AppSettingsService>();
            builder.Services.TryAddSingleton<ISettingsService<Setting>, SettingsService>();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}