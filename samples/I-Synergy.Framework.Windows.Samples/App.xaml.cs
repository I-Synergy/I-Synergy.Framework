using System;
using System.Collections.Generic;
using System.IO;
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
using Windows.Networking.Connectivity;

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

        public override async Task HandleException(Exception ex, string message)
        {
            var connections = NetworkInformation.GetInternetConnectionProfile();

            if (connections?.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.InternetAccess)
            {
                await ServiceLocator.Default.GetInstance<IDialogService>().ShowInformationAsync(
                    ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_DEFAULT_INTERNET"));
            }
            else
            {
                if (ex is NotImplementedException)
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowInformationAsync(
                        ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_FUTURE_MODULE"));
                }
                else if (ex is UnauthorizedAccessException accessException)
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(accessException.Message);
                }
                else if (ex is IOException iOException)
                {
                    if (iOException.Message.Contains("The process cannot access the file") && iOException.Message.Contains("because it is being used by another process"))
                    {
                        await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_FILEINUSE"));
                    }
                    else
                    {
                        await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                    }
                }
                else if (ex is ArgumentException argumentException)
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowWarningAsync(
                        string.Format(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_ARGUMENTNULL"),
                            argumentException.ParamName)
                        );
                }
                else
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                }
            }
        }
    }
}
