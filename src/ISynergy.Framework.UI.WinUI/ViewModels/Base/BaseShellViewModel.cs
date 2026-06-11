using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Windows;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ISynergy.Framework.UI.ViewModels.Base
{
    public abstract class BaseShellViewModel : ViewModelBladeView<NavigationItem>, IShellViewModel
    {
        protected readonly INavigationService _navigationService;
        protected readonly IDialogService _dialogService;
        protected readonly ISettingsService _settingsService;
        protected readonly IServiceScopeFactory _scopeFactory;

        public bool IsBackEnabled
        {
            get => GetValue<bool>();
            private set => SetValue(value);
        }

        public AsyncRelayCommand GoBackCommand { get; private set; }
        public AsyncRelayCommand RestartUpdateCommand { get; private set; }
        public AsyncRelayCommand SignInCommand { get; private set; }
        public AsyncRelayCommand LanguageCommand { get; private set; }
        public AsyncRelayCommand ThemeCommand { get; private set; }
        public AsyncRelayCommand HelpCommand { get; private set; }
        public AsyncRelayCommand SettingsCommand { get; private set; }
        public AsyncRelayCommand BackgroundCommand { get; private set; }
        public AsyncRelayCommand FeedbackCommand { get; private set; }

        public ObservableCollection<NavigationItem>? PrimaryItems
        {
            get => GetValue<ObservableCollection<NavigationItem>?>();
            set => SetValue(value);
        }

        public ObservableCollection<NavigationItem>? SecondaryItems
        {
            get => GetValue<ObservableCollection<NavigationItem>?>();
            set => SetValue(value);
        }

        protected BaseShellViewModel(ICommonServices commonServices, IDialogService dialogService, INavigationService navigationService, ISettingsService settingsService, IServiceScopeFactory scopeFactory, ILogger<BaseShellViewModel> logger)
        : base(commonServices, logger)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;
            _settingsService = settingsService;
            _scopeFactory = scopeFactory;

            _navigationService.BackStackChanged += NavigationService_BackStackChanged;
            IsBackEnabled = _navigationService.CanGoBack;

            PrimaryItems = new ObservableCollection<NavigationItem>();
            SecondaryItems = new ObservableCollection<NavigationItem>();

            GoBackCommand = new AsyncRelayCommand(async () => await _navigationService.GoBackAsync());
            RestartUpdateCommand = new AsyncRelayCommand(ShowDialogRestartAfterUpdateAsync);
            SignInCommand = new AsyncRelayCommand(SignOutAsync);
            LanguageCommand = new AsyncRelayCommand(OpenLanguageAsync);
            ThemeCommand = new AsyncRelayCommand(OpenThemesAsync);
            HelpCommand = new AsyncRelayCommand(OpenHelpAsync);
            FeedbackCommand = new AsyncRelayCommand(OpenFeedbackAsync);
            SettingsCommand = new AsyncRelayCommand(OpenSettingsAsync);
            BackgroundCommand = new AsyncRelayCommand(OpenBackgroundAsync);
        }

        public abstract Task ShellLoadedAsync();

        public abstract Task InitializeFirstRunAsync();


        private void NavigationService_BackStackChanged(object? sender, EventArgs e) =>
        IsBackEnabled = _navigationService.CanGoBack;

        protected abstract Task OpenSettingsAsync();

        protected abstract Task OpenBackgroundAsync();

        protected abstract Task SignOutAsync();

        protected Task ShowDialogRestartAfterUpdateAsync() =>
        _dialogService.ShowInformationAsync(_commonServices.LanguageService.GetString("UpdateRestart"));

        public NavigationItem LastSelectedItem
        {
            get => GetValue<NavigationItem>();
            set => SetValue(value);
        }

        public string Query
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string Caption
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool IsUpdateAvailable
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public double Width
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        protected virtual Task OpenLanguageAsync()
        {
            var scope = _scopeFactory.CreateScope();
            var languageVM = scope.ServiceProvider.GetRequiredService<LanguageViewModel>();
            languageVM.Submitted += LanguageVM_Submitted;
            languageVM.Closed += (_, _) =>
            {
                scope.Dispose();
            };
            return _dialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
        }

        private async void LanguageVM_Submitted(object? sender, SubmitEventArgs<CultureInfo> e)
        {
            try
            {
                if (sender is LanguageViewModel vm)
                {
                    vm.Submitted -= LanguageVM_Submitted;
                }

                _settingsService.LocalSettings.Language = e.Result.Name;
                _settingsService.SaveLocalSettings();

                e.Result.SetLocalizationLanguage();

                if (await _dialogService.ShowMessageAsync(
                _commonServices.LanguageService.GetString("WarningLanguageChange") +
                Environment.NewLine +
                _commonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                _commonServices.LanguageService.GetString("TitleQuestion"),
                MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
                {
                    _commonServices.RestartApplication();
                }
            }
            catch (Exception ex)
            {
                // Log exception to prevent it from being swallowed in async void method
                _logger?.LogError(ex, "Error in LanguageVM_Submitted event handler");
                var exceptionHandlerService = _commonServices?.ExceptionHandlerService;
                exceptionHandlerService?.HandleException(ex);
                // Re-throw to crash the application if unhandled exception handler is configured
                throw;
            }
        }

        protected virtual Task OpenThemesAsync()
        {
            var scope = _scopeFactory.CreateScope();
            var themeVM = scope.ServiceProvider.GetRequiredService<ThemeViewModel>();
            var style = new ThemeStyle
            {
                Theme = _settingsService.LocalSettings.Theme,
                Color = _settingsService.LocalSettings.Color
            };
            themeVM.Submitted += ThemeVM_Submitted;
            themeVM.SetSelectedItem(style);
            themeVM.Closed += (_, _) =>
            {
                scope.Dispose();
            };
            return _dialogService.ShowDialogAsync(typeof(IThemeWindow), themeVM);
        }

        private async void ThemeVM_Submitted(object? sender, SubmitEventArgs<ThemeStyle> e)
        {
            try
            {
                if (sender is ThemeViewModel vm)
                {
                    vm.Submitted -= ThemeVM_Submitted;

                    _settingsService.LocalSettings.Theme = e.Result.Theme;
                    _settingsService.LocalSettings.Color = (e.Result.Color ?? string.Empty).ToLowerInvariant();

                    if (_settingsService.SaveLocalSettings() &&
                    await _dialogService.ShowMessageAsync(
                    _commonServices.LanguageService.GetString("WarningColorChange") +
                    Environment.NewLine +
                    _commonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                    _commonServices.LanguageService.GetString("TitleQuestion"),
                    MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
                    {
                        _commonServices.RestartApplication();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception to prevent it from being swallowed in async void method
                _logger?.LogError(ex, "Error in ThemeVM_Submitted event handler");
                var exceptionHandlerService = _commonServices?.ExceptionHandlerService;
                exceptionHandlerService?.HandleException(ex);
                // Re-throw to crash the application if unhandled exception handler is configured
                throw;
            }
        }

        /// <summary>
        /// Opens the help asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Default implementation does nothing. Override in derived classes to provide help functionality.
        /// </remarks>
        protected virtual Task OpenHelpAsync() => Task.CompletedTask;

        /// <summary>
        /// Opens the feedback dialog asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Default implementation does nothing. Override in derived classes to provide feedback functionality.
        /// </remarks>
        protected virtual Task OpenFeedbackAsync() => Task.CompletedTask;

        /// <summary>
        /// Adds a new item asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Default implementation does nothing. Override in derived classes to provide add functionality.
        /// </remarks>
        public override Task AddAsync() => Task.CompletedTask;

        /// <summary>
        /// Edits the specified item asynchronously.
        /// </summary>
        /// <param name="e">The item to edit.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Default implementation does nothing. Override in derived classes to provide edit functionality.
        /// </remarks>
        public override Task EditAsync(NavigationItem e) => Task.CompletedTask;

        /// <summary>
        /// Removes the specified item asynchronously.
        /// </summary>
        /// <param name="e">The item to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Default implementation does nothing. Override in derived classes to provide remove functionality.
        /// </remarks>
        public override Task RemoveAsync(NavigationItem e) => Task.CompletedTask;

        /// <summary>
        /// Searches for items asynchronously.
        /// </summary>
        /// <param name="e">The search criteria.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Default implementation does nothing. Override in derived classes to provide search functionality.
        /// </remarks>
        public override Task SearchAsync(object e) => Task.CompletedTask;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_navigationService is not null)
                {
                    _navigationService.BackStackChanged -= NavigationService_BackStackChanged;
                }

                Validator = null;

                PrimaryItems = null;
                SecondaryItems = null;

                GoBackCommand?.Dispose();
                RestartUpdateCommand?.Dispose();
                SignInCommand?.Dispose();
                LanguageCommand?.Dispose();
                ThemeCommand?.Dispose();
                HelpCommand?.Dispose();
                SettingsCommand?.Dispose();
                BackgroundCommand?.Dispose();
                FeedbackCommand?.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}
