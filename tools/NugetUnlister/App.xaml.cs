using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Logging.Extensions;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Abstractions.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NugetUnlister.Common.Abstractions;
using NugetUnlister.Common.Extensions;
using NugetUnlister.Options;
using NugetUnlister.Properties;
using NugetUnlister.Services;
using NugetUnlister.ViewModels;
using NugetUnlister.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace NugetUnlister
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : BaseApplication
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
            Application.Current.MainWindow = new Window() { Title = "I-Synergy Nuget Unlister" };
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

            services.AddSingleton<IVersionService>((s) => new VersionService(assembly));
            services.AddSingleton<IInfoService>((s) => new InfoService(assembly));
            services.AddSingleton<IContext, Context>();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, AppSettingsService>());

            services.AddLoggingAppCenterIntegration(ConfigurationRoot);
            services.AddNugetServiceIntegrations(ConfigurationRoot);

            // Load Syncfusion license key.
            ConfigurationRoot.GetSection(nameof(SyncfusionLicenseOptions)).Bind(_syncfusionLicenseOptions);
            services.AddSingleton((s) => _syncfusionLicenseOptions);
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(_syncfusionLicenseOptions.LicenseKey);

            services.AddSingleton<IShellViewModel, ShellViewModel>();
            services.AddSingleton<IShellView, ShellView>();

            AddResourceManager(new ResourceManager(typeof(Resources)));

            //Add current assembly.
            RegisterAssemblies(assembly, x =>
                x.Name.StartsWith(GetType().Namespace) ||
                x.Name.Equals(Assembly.GetAssembly(typeof(NugetUnlister.Common.Assembly.Identifier)).FullName));
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
