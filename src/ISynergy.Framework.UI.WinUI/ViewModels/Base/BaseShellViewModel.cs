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
    private readonly ShellViewModelSupport _support;

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
        _support = new ShellViewModelSupport(navigationService, themeService, dialogService, commonServices, value => IsBackEnabled = value);

            PrimaryItems = new ObservableCollection<NavigationItem>();
            SecondaryItems = new ObservableCollection<NavigationItem>();

            GoBackCommand = new AsyncRelayCommand(async () => await _navigationService.GoBackAsync());
            RestartUpdateCommand = new AsyncRelayCommand(_support.ShowRestartAsync);
            SignInCommand = new AsyncRelayCommand(SignOutAsync);
            ThemeCommand = new AsyncRelayCommand(_support.ToggleThemeAsync);
            HelpCommand = new AsyncRelayCommand(OpenHelpAsync);
            FeedbackCommand = new AsyncRelayCommand(OpenFeedbackAsync);
            SettingsCommand = new AsyncRelayCommand(OpenSettingsAsync);
            BackgroundCommand = new AsyncRelayCommand(OpenBackgroundAsync);
        }

        public abstract Task ShellLoadedAsync();

        public abstract Task InitializeFirstRunAsync();


        protected abstract Task OpenSettingsAsync();

        protected abstract Task OpenBackgroundAsync();

        protected abstract Task SignOutAsync();


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
                _support.Dispose();

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
