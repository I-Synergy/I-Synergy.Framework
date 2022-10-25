using ISynergy.Framework.Clipboard.Extensions;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Telemetry.Extensions;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.Update.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Sample.Abstractions.Services;
using Sample.Options;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Reflection;
using System.Resources;
using System.Web;
using Windows.ApplicationModel.Activation;

namespace Sample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : BaseApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
            : base()
        {
            InitializeComponent();
        }

        protected override void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            var context = ServiceLocator.Default.GetInstance<IContext>();
            var activationArguments = AppInstance.GetCurrent().GetActivatedEventArgs();

            if (activationArguments.Kind == ExtendedActivationKind.Launch && activationArguments.Data is ILaunchActivatedEventArgs launchArguments)
            {
                var environmentOption = new Option<SoftwareEnvironments>(
                name: "--environment",
                description: "The environment (Local, Test or Production) to use.",
                getDefaultValue: () => SoftwareEnvironments.Production);

                var environmentCommand = new RootCommand();
                environmentCommand.AddOption(environmentOption);
                environmentCommand.AddAlias("--Environment");
                environmentCommand.AddAlias("-e");
                environmentCommand.SetHandler((environment) =>
                {
                    context.Environment = environment;
                }, environmentOption);

                environmentCommand.Invoke(launchArguments.Arguments);
            }
            else if (activationArguments.Kind == ExtendedActivationKind.Protocol && activationArguments.Data is IProtocolActivatedEventArgs protocolArguments)
            {
                var env = HttpUtility.ParseQueryString(protocolArguments.Uri.Query).Get("environment");
                
                if (!string.IsNullOrEmpty(env))
                    context.Environment = env.ToEnum<SoftwareEnvironments>(SoftwareEnvironments.Production);
            }
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

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            var assembly = Assembly.GetAssembly(typeof(App));

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonStream(assembly.GetManifestResourceStream("appsettings.json"))
                .Build();

            services.Configure<ConfigurationOptions>(configurationRoot.GetSection(nameof(ConfigurationOptions)).BindWithReload);

            services.AddSingleton<IVersionService>((s) => new VersionService(assembly));
            services.AddSingleton<IInfoService>((s) => new InfoService(assembly));
            services.AddSingleton<IContext, Context>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            services.AddUpdatesIntegration();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, AppSettingsService>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService, SettingsService>());

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

            services.AddAppCenterTelemetryIntegration(configurationRoot);

            services.AddClipboardIntegration();

            services.AddScoped<IShellViewModel, ShellViewModel>();
            services.AddScoped<IShellView, ShellView>();

            //Add current resource manager.
            AddResourceManager(new ResourceManager(typeof(Resources)));

            //Add current assembly.
            //Add current assembly.
            RegisterAssemblies(assembly, x =>
                x.Name.StartsWith(GetType().Namespace));
        }
    }
}
