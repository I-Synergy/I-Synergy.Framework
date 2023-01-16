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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Abstractions.Services;
using Sample.Options;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using System.Reflection;
using System.Resources;

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

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, AppSettingsService>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService, SettingsService>());

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

            //services.AddTelemetrySentryIntegration(configurationRoot);
            //services.AddApplicationInsightsTelemetryIntegration(configurationRoot);
            services.AddSentryTelemetryIntegration(configurationRoot);

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
