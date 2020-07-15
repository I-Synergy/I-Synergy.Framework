using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Windows.Abstractions.Views;
using ISynergy.Framework.Windows.Samples.Abstractions.Services;
using ISynergy.Framework.Windows.Samples.Options;
using ISynergy.Framework.Windows.Samples.Services;
using ISynergy.Framework.Windows.Samples.ViewModels;
using ISynergy.Framework.Windows.Samples.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel;

namespace ISynergy.Framework.Windows.Samples
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : BaseApplication
    {
        /// <summary>
        /// The configuration root
        /// </summary>
        private IConfigurationRoot ConfigurationRoot;
        /// <summary>
        /// The configuration options
        /// </summary>
        private ConfigurationOptions _configurationOptions = new ConfigurationOptions();
        /// <summary>
        /// The application center options
        /// </summary>
        private AppCenterOptions _appCenterOptions = new AppCenterOptions();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() : base() { }

        protected override void ConfigureLogger(ILoggerFactory factory)
        {
        }

        protected override void ConfigureServices()
        {
            base.ConfigureServices();

            var applicationExecutingPath = Package.Current.Installed­Location.Path;

            ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile($"{applicationExecutingPath}\\appsettings.json", false)
                .Build();

            ConfigurationRoot.GetSection(nameof(ConfigurationOptions)).Bind(_configurationOptions);
            ServiceLocator.Default.Register(() => _configurationOptions);

            ServiceLocator.Default.Register<IContext, Context>();
            ServiceLocator.Default.Register<IAuthenticationService, AuthenticationService>();
            ServiceLocator.Default.Register<IUpdateService, UpdateService>();
            ServiceLocator.Default.Register<ILanguageService, LanguageService>();
            ServiceLocator.Default.Register<ISettingsService, SettingsService>();
            ServiceLocator.Default.Register<ICommonServices, CommonServices>();
            ServiceLocator.Default.Register<IBaseCommonServices>(() => ServiceLocator.Default.GetInstance<ICommonServices>());

            ServiceLocator.Default.Register<ITelemetryService, TelemetryService>();
            ServiceLocator.Default.Register<IMasterDataService, MasterDataService>();
            ServiceLocator.Default.Register<IClientMonitorService, ClientMonitorService>();
            ServiceLocator.Default.Register<IFileService, FileService>();
            ServiceLocator.Default.Register<ICameraService, CameraService>();
            ServiceLocator.Default.Register<IClipboardService, ClipboardService>();
            ServiceLocator.Default.Register<IDownloadFileService, DownloadFileService>();
            ServiceLocator.Default.Register<IReportingService, ReportingService>();
            ServiceLocator.Default.Register<IPrintingService, PrintingService>();

            ServiceLocator.Default.Register<IShellViewModel, ShellViewModel>();
            ServiceLocator.Default.Register<IShellView, ShellView>();

            //Load assemblies
            RegisterAssemblies(new List<Assembly>
                {
                    Assembly.Load("I-Synergy.Framework.Windows.Samples")
                });
        }

        public override Task HandleException(Exception ex, string message)
        {
            throw new NotImplementedException();
        }
    }
}
