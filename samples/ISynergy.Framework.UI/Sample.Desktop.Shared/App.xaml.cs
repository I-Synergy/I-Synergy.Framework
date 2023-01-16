using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Telemetry.Extensions;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.Update.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.UI.Xaml;
using Sample.Abstractions.Services;
using Sample.Options;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Sample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : BaseApplication
    {
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
            : base()
        {
            InitializeComponent();

            _services = new ServiceCollection();
            _services.ConfigureServices<App, Context, Sample.Properties.Resources>(x => x.Name.StartsWith(typeof(App).Namespace));

            _services.AddSingleton<IAuthenticationService, AuthenticationService>();

            _services.AddUpdatesIntegration();

            _services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, AppSettingsService>());
            _services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService, SettingsService>());

            _services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
            _services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

            //_services.AddAppCenterTelemetryIntegration(configurationRoot);

            _services.AddScoped<IShellViewModel, ShellViewModel>();
            _services.AddScoped<IShellView, ShellView>();

            _services.BuildServiceProvider();

            InitializeApplication();
        }

        /// <summary>
        /// Add additional resource dictionaries.
        /// </summary>
        /// <returns></returns>
        protected override IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>()
            {
                new ResourceDictionary() { Source = new Uri("ms-appx:///Styles/Style.Desktop.xaml") }
            };
    }
}
