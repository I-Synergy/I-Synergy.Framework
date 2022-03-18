using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Functions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class BaseApplication.
    /// Implements the <see cref="Application" />
    /// </summary>
    /// <seealso cref="Application" />
    public abstract partial class BaseApplication : Application
    {
        /// <summary>
        /// Handles the UnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void Current_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            HandleException(e.Exception, e.Exception.Message);
        }

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var window = Application.Current.MainWindow;
            var rootFrame = MainWindow.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame is null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Add custom resourcedictionaries from code.
                var dictionary = Application.Current.Resources?.MergedDictionaries;

                if (dictionary is not null)
                {
                    foreach (var item in GetAdditionalResourceDictionaries())
                    {
                        if (!dictionary.Any(t => t.Source == item.Source))
                            Application.Current.Resources.MergedDictionaries.Add(item);
                    }
                }

                // Place the frame in the current Window
                MainWindow.Content = rootFrame;
            }

            if (rootFrame.Content is null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                var view = _serviceProvider.GetRequiredService<IShellView>();
                rootFrame.Navigate(view, e.Args);
            }

            _themeSelector = _serviceProvider.GetRequiredService<IThemeService>();
            _themeSelector.InitializeMainWindow(MainWindow);

            // Ensure the current window is active
            MainWindow.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load {e.Uri}: {e.Exception}");
        }

        /// <summary>
        /// Get a new list of additional resource dictionaries which can be merged.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>();

        /// <summary>
        /// Gets the name of the descendant from.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <returns>FrameworkElement.</returns>
        public static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
                return null;

            for (var i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.Name == name)
                        return frameworkElement;

                    frameworkElement = GetDescendantFromName(frameworkElement, name);

                    if (frameworkElement != null)
                        return frameworkElement;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        public virtual void Initialize()
        {
            Current.DispatcherUnhandledException += Current_UnhandledException;

            _context = _serviceProvider.GetRequiredService<IContext>();
            _context.ViewModels = ViewModelTypes;

            //Only in Windows i can set the culture.
            //var culture = CultureInfo.CurrentCulture;

            //culture.NumberFormat.CurrencySymbol = $"{_context.CurrencySymbol} ";
            //culture.NumberFormat.CurrencyNegativePattern = 1;

            //_context.NumberFormat = culture.NumberFormat;

            //var localizationFunctions = _serviceProvider.GetRequiredService<LocalizationFunctions>();
            //localizationFunctions.SetLocalizationLanguage(_serviceProvider.GetRequiredService<IBaseApplicationSettingsService>().Settings.Culture);

            // Bootstrap all registered modules.
            foreach (var bootstrapper in BootstrapperTypes.Distinct())
                if (_serviceProvider.GetService(bootstrapper) is IBootstrap instance)
                    instance.Bootstrap();
        }
    }
}
