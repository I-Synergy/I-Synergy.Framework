using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Windows;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.UI.ViewModels.Base
{
 public abstract class BaseShellViewModel : ViewModelBladeView<NavigationItem>, IShellViewModel
 {
 protected readonly INavigationService _navigationService;
 protected readonly IDialogService _dialogService;

 public bool IsBackEnabled
 {
 get => GetValue<bool>();
 private set => SetValue(value);
 }

 public AsyncRelayCommand GoBackCommand { get; private set; }
 public AsyncRelayCommand RestartUpdateCommand { get; private set; }
 public AsyncRelayCommand SignInCommand { get; private set; }
 public AsyncRelayCommand LanguageCommand { get; private set; }
 public AsyncRelayCommand ColorCommand { get; private set; }
 public AsyncRelayCommand HelpCommand { get; private set; }
 public AsyncRelayCommand SettingsCommand { get; private set; }
 public AsyncRelayCommand BackgroundCommand { get; private set; }
 public AsyncRelayCommand FeedbackCommand { get; private set; }

 public ObservableCollection<NavigationItem> PrimaryItems
 {
 get => GetValue<ObservableCollection<NavigationItem>>();
 set => SetValue(value);
 }

 public ObservableCollection<NavigationItem> SecondaryItems
 {
 get => GetValue<ObservableCollection<NavigationItem>>();
 set => SetValue(value);
 }

 protected BaseShellViewModel(ICommonServices commonServices, IDialogService dialogService, INavigationService navigationService, ILogger<BaseShellViewModel> logger)
 : base(commonServices, logger)
 {
 _dialogService = dialogService;
 _navigationService = navigationService;

 _navigationService.BackStackChanged += NavigationService_BackStackChanged;
 IsBackEnabled = _navigationService.CanGoBack;

 PrimaryItems = new ObservableCollection<NavigationItem>();
 SecondaryItems = new ObservableCollection<NavigationItem>();

 GoBackCommand = new AsyncRelayCommand(async () => await _navigationService.GoBackAsync());
 RestartUpdateCommand = new AsyncRelayCommand(ShowDialogRestartAfterUpdateAsync);
 SignInCommand = new AsyncRelayCommand(SignOutAsync);
 LanguageCommand = new AsyncRelayCommand(OpenLanguageAsync);
 ColorCommand = new AsyncRelayCommand(OpenColorsAsync);
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
 var languageVM = _commonServices.ScopedContextService.GetRequiredService<LanguageViewModel>();
 languageVM.Submitted += LanguageVM_Submitted;
 return _dialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
 }

 private async void LanguageVM_Submitted(object? sender, SubmitEventArgs<Languages> e)
 {
 if (sender is LanguageViewModel vm)
 vm.Submitted -= LanguageVM_Submitted;

 _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Language = e.Result;
 _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings();

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

 protected virtual Task OpenColorsAsync()
 {
 var themeVM = _commonServices.ScopedContextService.GetRequiredService<ThemeViewModel>();
 themeVM.Submitted += ThemeVM_Submitted;
 return _dialogService.ShowDialogAsync(typeof(IThemeWindow), themeVM);
 }

 private async void ThemeVM_Submitted(object? sender, SubmitEventArgs<ThemeStyle> e)
 {
 if (sender is ThemeViewModel vm)
 {
 vm.Submitted -= ThemeVM_Submitted;

 _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Theme = e.Result.Theme;
 _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color = (e.Result.Color ?? string.Empty).ToLowerInvariant();

 if (_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings() &&
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

 protected virtual Task OpenHelpAsync() =>
 throw new NotImplementedException();

 protected virtual Task OpenFeedbackAsync() =>
 throw new NotImplementedException();

 public override Task AddAsync() =>
 throw new NotImplementedException();

 public override Task EditAsync(NavigationItem e) =>
 throw new NotImplementedException();

 public override Task RemoveAsync(NavigationItem e) =>
 throw new NotImplementedException();

 public override Task SearchAsync(object e) =>
 throw new NotImplementedException();

 protected override void Dispose(bool disposing)
 {
 if (disposing)
 {
 if (_navigationService is not null)
 _navigationService.BackStackChanged -= NavigationService_BackStackChanged;

 Validator = null;

 PrimaryItems?.Clear();
 SecondaryItems?.Clear();

 GoBackCommand?.Dispose();
 RestartUpdateCommand?.Dispose();
 SignInCommand?.Dispose();
 LanguageCommand?.Dispose();
 ColorCommand?.Dispose();
 HelpCommand?.Dispose();
 SettingsCommand?.Dispose();
 BackgroundCommand?.Dispose();
 FeedbackCommand?.Dispose();

 base.Dispose(disposing);
 }
 }
 }
}
