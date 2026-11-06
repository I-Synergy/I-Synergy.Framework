using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.ViewModels.Base;

/// <summary>
/// Shared shell behaviour used by the platform shell view models: back-stack tracking,
/// theme toggling and the "restart after update" dialog.
/// </summary>
/// <remarks>
/// The platform shell view models derive from different Mvvm base classes
/// (<c>ViewModel</c>, <c>ViewModelNavigation&lt;T&gt;</c>, <c>ViewModelBladeView&lt;T&gt;</c>),
/// so this composition helper carries the common implementation instead of a shared base class.
/// </remarks>
public sealed class ShellViewModelSupport : IDisposable
{
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly IDialogService _dialogService;
    private readonly ICommonServices _commonServices;
    private readonly Action<bool> _setIsBackEnabled;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModelSupport"/> class and starts
    /// tracking the navigation back stack.
    /// </summary>
    /// <param name="navigationService">The navigation service whose back stack is tracked.</param>
    /// <param name="themeService">The theme service used to toggle light/dark.</param>
    /// <param name="dialogService">The dialog service used for the restart prompt.</param>
    /// <param name="commonServices">The common services used for localized strings.</param>
    /// <param name="setIsBackEnabled">Callback that applies the current back-stack state.</param>
    public ShellViewModelSupport(
        INavigationService navigationService,
        IThemeService themeService,
        IDialogService dialogService,
        ICommonServices commonServices,
        Action<bool> setIsBackEnabled)
    {
        _navigationService = navigationService;
        _themeService = themeService;
        _dialogService = dialogService;
        _commonServices = commonServices;
        _setIsBackEnabled = setIsBackEnabled;

        _navigationService.BackStackChanged += OnBackStackChanged;
        _setIsBackEnabled(_navigationService.CanGoBack);
    }

    /// <summary>
    /// Toggles between the light and dark theme and applies it immediately.
    /// </summary>
    public Task ToggleThemeAsync()
    {
        var nextTheme = _themeService.Theme == Themes.Light ? Themes.Dark : Themes.Light;
        _themeService.SetTheme(nextTheme);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Shows the "restart required after update" dialog.
    /// </summary>
    public Task ShowRestartAsync() =>
        _dialogService.ShowInformationAsync(_commonServices.LanguageService.GetString("UpdateRestart"));

    private void OnBackStackChanged(object? sender, EventArgs e) =>
        _setIsBackEnabled(_navigationService.CanGoBack);

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _navigationService.BackStackChanged -= OnBackStackChanged;
    }
}
