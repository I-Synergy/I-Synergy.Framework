﻿using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sample.Abstractions.Services;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using System.Reflection;
using HostBuilder = Microsoft.Extensions.Hosting.HostBuilder;

namespace Sample
{
    public class App : BaseApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
            : base()
        {
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    Assembly mainAssembly = Assembly.GetAssembly(typeof(App));
                    builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
                })
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureServices<App, Context, ExceptionHandlerService, Sample.Properties.Resources>(context.Configuration, x => x.Name.StartsWith(typeof(App).Namespace));

                    services.TryAddSingleton<IAuthenticationService, AuthenticationService>();

                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, AppSettingsService>());
                    services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService<Setting>, SettingsService>());

                    services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
                    services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());
                })
                .ConfigureLogging((context, logging) =>
                {
#if __WASM__
                    logging.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__ || __MACCATALYST__
                    logging.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                    logging.AddDebug();
#else
                    logging.AddConsole();
#endif

                    // Exclude logs below this level
                    logging.SetMinimumLevel(LogLevel.Information);

                    // Default filters for Uno Platform namespaces
                    logging.AddFilter("Uno", LogLevel.Warning);
                    logging.AddFilter("Windows", LogLevel.Warning);
                    logging.AddFilter("Microsoft", LogLevel.Warning);
                });
        }

        protected override IList<ResourceDictionary> GetAdditionalResourceDictionaries() => new List<ResourceDictionary>()
        {
            new XamlControlsResources(),
            new Sample.Assets.Images(),
            new Sample.Styles.Style()
        };

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            base.OnLaunched(e);
            await ServiceLocator.Default.GetInstance<INavigationService>().NavigateModalAsync<AuthenticationViewModel>();
        }

        protected override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
        {
            if (ServiceLocator.Default.GetInstance<INavigationService>() is NavigationService navigationService)
            {
                await navigationService.CleanBackStackAsync();

                if (e.Value)
                {
                    await navigationService.NavigateModalAsync<ShellViewModel>();
                }
                else
                {
                    await navigationService.NavigateModalAsync<AuthenticationViewModel>();
                }
            }
        }
    }
}