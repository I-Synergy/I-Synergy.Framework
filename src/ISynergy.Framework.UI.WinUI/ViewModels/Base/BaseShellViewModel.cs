using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.UI.ViewModels.Base
{
    public abstract class BaseShellViewModel : ViewModelBladeView<NavigationItem>, IShellViewModel
    {
        protected readonly INavigationService _navigationService;
        protected readonly IDialogService _dialogService;
        protected readonly ISettingsService _settingsService;
        protected readonly IThemeService _themeService;

        public bool IsBackEnabled
        {
            get => GetValue<bool>();
            private set => SetValue(value);
        }

        public AsyncRelayCommand GoBackCommand { get; private set; }
        public AsyncRelayCommand RestartUpdateCommand { get; private set; }
        public AsyncRelayCommand SignInCommand { get; private set; }
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

        protected BaseShellViewModel(ICommonServices commonServices, IDialogService dialogService, INavigationService navigationService, ISettingsService settingsService, IThemeService themeService, ILogger<BaseShellViewModel> logger)
        : base(commonServices, logger)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;
            _settingsService = settingsService;
            _themeService = themeService;

            _navigationService.BackStackChanged += NavigationService_BackStackChanged;
            IsBackEnabled = _navigationService.CanGoBack;

            PrimaryItems = new ObservableCollection<NavigationItem>();
            SecondaryItems = new ObservableCollection<NavigationItem>();

            GoBackCommand = new AsyncRelayCommand(async () => await _navigationService.GoBackAsync());
            RestartUpdateCommand = new AsyncRelayCommand(ShowDialogRestartAfterUpdateAsync);
            SignInCommand = new AsyncRelayCommand(SignOutAsync);
            ThemeCommand = new AsyncRelayCommand(ToggleThemeAsync);
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

        /// <summary>
        /// Toggles between the light and dark theme and applies it immediately.
        /// Accent color selection lives in the application's settings UI.
        /// </summary>
        protected Task ToggleThemeAsync()
        {
            var nextTheme = _themeService.Theme == Themes.Light ? Themes.Dark : Themes.Light;
            _themeService.SetTheme(nextTheme);
            return Task.CompletedTask;
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
