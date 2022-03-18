using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Abstractions.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Abstractions;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using Sample.Options;
using Sample.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Windows;
using ISynergy.Framework.Telemetry.Extensions;
using ISynergy.Framework.Clipboard.Extensions;

namespace Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App :  BaseApplication
    {
        /// <summary>
        /// The configuration root
        /// </summary>
        private IConfigurationRoot ConfigurationRoot;

        /// <summary>
        /// The syncfusion license options
        /// </summary>
        private SyncfusionLicenseOptions _syncfusionLicenseOptions = new SyncfusionLicenseOptions();

        /// <summary>
        /// Constructor of the application
        /// </summary>
        public App() : base()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Application.Current.MainWindow = new Window() { Title = "I-Synergy WPF UI Framework Sample" };
            base.OnStartup(e);
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            var assembly = Assembly.GetAssembly(typeof(App));

            ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddSingleton<IInfoService>((s) => new InfoService(assembly));
            services.AddSingleton<IContext, Context>();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, AppSettingsService>());

            services.AddTelemetryAppCenterIntegration(ConfigurationRoot);
            services.AddClipboardIntegration();
            services.AddSingleton<IUnitConversionService, UnitConversionService>();

            // Load Syncfusion license key.
            ConfigurationRoot.GetSection(nameof(SyncfusionLicenseOptions)).Bind(_syncfusionLicenseOptions);
            services.AddSingleton((s) => _syncfusionLicenseOptions);
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(_syncfusionLicenseOptions.LicenseKey);

            services.AddSingleton<IShellViewModel, ShellViewModel>();
            services.AddSingleton<IShellView, ShellView>();

            AddResourceManager(new ResourceManager(typeof(Resources)));

            //Load assemblies
            RegisterAssemblies(new List<Assembly>
                {
                    assembly
                });
        }

        public override void HandleException(Exception exception, string message)
        {
            try
            {
                Debug.WriteLine(message);

                // Set busyIndicator to false if it's true.
                ServiceLocator.Default.GetInstance<IBusyService>().EndBusy();
                ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(exception.Message).Await();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message, ex);
            }
        }
    }
}
