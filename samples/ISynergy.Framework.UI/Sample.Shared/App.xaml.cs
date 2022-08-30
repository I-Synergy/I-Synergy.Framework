using ISynergy.Framework.Clipboard.Extensions;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Telemetry.ApplicationInsights.Extensions;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.Update.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Sample.Abstractions.Services;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
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

            //services.AddTelemetrySentryIntegration(configurationRoot);
            services.AddApplicationInsightsTelemetryIntegration(configurationRoot);

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

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public override void HandleException(Exception exception, string message)
        {
            try
            {
                _telemetryService.TrackException(exception, message);

                _logger.LogDebug(message);

                if(exception.InnerException is WebSocketException)
                    return;

                // Set busyIndicator to false if it's true.
                ServiceLocator.Default.GetInstance<IBusyService>().EndBusy();

                if (exception is UnauthorizedAccessException accessException)
                {
                    ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(accessException.Message).Await();
                }
                else if (exception is IOException iOException)
                {
                    if (iOException.Message.Contains("The process cannot access the file") && iOException.Message.Contains("because it is being used by another process"))
                    {
                        ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_FILEINUSE")).Await();
                    }
                    else
                    {
                        ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(iOException.Message).Await();
                    }
                }
                else if (exception is ArgumentException argumentException)
                {
                    ServiceLocator.Default.GetInstance<IDialogService>().ShowWarningAsync(
                        string.Format(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_ARGUMENTNULL"),
                            argumentException.ParamName)
                        ).Await();
                }
                else
                {
                    ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(exception.Message).Await();
                }
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex.Message, ex);
            }
            finally
            {
                _telemetryService.Flush();
            }
        }
    }
}
