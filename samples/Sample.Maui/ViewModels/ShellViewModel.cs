using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Sample.ViewModels;

public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
    /// <summary>
    /// Gets or sets the Version property value.
    /// </summary>
    /// <value>The version.</value>
    public Version Version
    {
        get { return GetValue<Version>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="dialogService"></param>
    /// <param name="navigationService"></param>
    /// <param name="settingsService"></param>
    /// <param name="logger">The logger factory.</param>
    public ShellViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ISettingsService settingsService,
        ILogger<ShellViewModel> logger)
        : base(commonServices, dialogService, navigationService, settingsService, logger)
    {
        Title = commonServices.InfoService.ProductName;
        Version = commonServices.InfoService.ProductVersion;
    }

    public override Task ShellLoadedAsync() =>
        InitializeFirstRunAsync();

    protected override Task SignOutAsync() =>
        ServiceLocator.Default.GetRequiredService<Sample.Abstractions.Services.IAuthenticationService>().SignOutAsync();

    public override async Task InitializeFirstRunAsync()
    {
        if (_settingsService.GlobalSettings!.IsFirstRun)
        {
            if (await _dialogService.ShowMessageAsync(
                _commonServices.LanguageService.GetString("ChangeLanguage"),
                _commonServices.LanguageService.GetString("Language"),
                MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
            {
                await _dialogService.ShowDialogAsync<ILanguageWindow, LanguageViewModel, CultureInfo>(
                    async culture =>
                    {
                        _settingsService.LocalSettings.Language = culture.Name;
                        _settingsService.SaveLocalSettings();
                        culture.SetLocalizationLanguage();
                        _commonServices.RestartApplication();
                    });
            }

            //using var settingsScope = _scopeFactory.CreateScope();
            //var wizardVM = settingsScope.ServiceProvider.GetRequiredService<SettingsViewModel>();
            //wizardVM.Submitted += (s, e) =>
            //{
            //    _commonServices.RestartApplication();
            //};
            //wizardVM.Cancelled += (s, e) =>
            //{
            //    _commonServices.RestartApplication();
            //};

            //await _navigationService.OpenBladeAsync(this, wizardVM);
        }
    }

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() =>
        _navigationService.NavigateModalAsync<SettingsViewModel>();

    /// <summary>
    /// Opens the help page as a modal.
    /// </summary>
    protected override Task OpenHelpAsync() =>
        _navigationService.NavigateModalAsync<HelpViewModel>();
}

