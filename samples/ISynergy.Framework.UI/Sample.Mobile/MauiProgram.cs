using CommunityToolkit.Maui;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Logging.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Views;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using System.Reflection;

namespace Sample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            var mainAssembly = Assembly.GetAssembly(typeof(App));

            var config = new ConfigurationBuilder()
                .AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"))
                .Build();

            builder.Configuration.AddConfiguration(config);

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FontAwesome.otf", "FontAwesome");
                })
                .ConfigureServices<App, Context, Properties.Resources>(x => x.Name.StartsWith(typeof(MauiProgram).Namespace));
            
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

            builder.Services.AddSingleton<IBaseApplicationSettingsService, AppSettingsService>();
            builder.Services.AddSingleton<ISettingsService<Setting>, SettingsService>();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

            builder.Services.AddSingleton<IAuthenticationView, AuthenticationView>();
            builder.Services.AddSingleton<IRegistrationView, RegistrationView>();

            builder.Services.AddSingleton<IShellViewModel, AppShellViewModel>();
            builder.Services.AddSingleton<IShellView, AppShell>();

            builder.Logging.AddAppCenterLogging(builder.Configuration);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}