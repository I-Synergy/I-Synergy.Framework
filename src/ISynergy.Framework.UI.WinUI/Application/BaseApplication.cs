using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Functions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

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
        /// Main Application Window.
        /// </summary>
        public Window MainWindow { get; private set; }

        /// <summary>
        /// Handles the UnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            HandleException(e.Exception, $"{e.Exception.Message}{Environment.NewLine}{e.Message}");
        }

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e) =>
            OnLaunchApplication(e);

        /// <summary>
        /// On launch of application.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnLaunchApplication(LaunchActivatedEventArgs e)
        {
            MainWindow = new Window();

            var rootFrame = MainWindow.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame is null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

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

            if (e.UWPLaunchActivatedEventArgs.Kind == ActivationKind.Launch)
            {
                if (rootFrame.Content is null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    var view = _serviceProvider.GetRequiredService<IShellView>();
                    rootFrame.Navigate(view.GetType(), e.Arguments);
                }
            }

            _themeSelector = _serviceProvider.GetRequiredService<IThemeService>();
            _themeSelector.InitializeMainWindow(MainWindow);

            MainWindow.Activate();
        }

        /// <summary>
        /// Get a new list of additional resource dictionaries which can be merged.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>();

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        /// <exception cref="Exception">Failed to load Page " + e.SourcePageType.FullName</exception>
        /// <exception cref="Exception">Failed to load Page " + e.SourcePageType.FullName</exception>
        private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e) =>
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");

        /// <summary>
        /// Sets the context.
        /// </summary>
        public virtual void Initialize()
        {
            Current.UnhandledException += Current_UnhandledException;

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            _context = _serviceProvider.GetRequiredService<IContext>();
            _context.ViewModels = ViewModelTypes;

            var localizationFunctions = _serviceProvider.GetRequiredService<LocalizationFunctions>();
            localizationFunctions.SetLocalizationLanguage(_serviceProvider.GetRequiredService<IBaseApplicationSettingsService>().Settings.Culture);

            // Bootstrap all registered modules.
            foreach (var bootstrapper in BootstrapperTypes.Distinct())
                if (_serviceProvider.GetService(bootstrapper) is IBootstrap instance)
                    instance.Bootstrap();
        }
    }
}
