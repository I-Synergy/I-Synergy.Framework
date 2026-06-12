using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailBViewModel : ViewModelBlade<TestItem>
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IBladeService _bladeService;

    public AsyncRelayCommand OpenNewBladeCommand { get; private set; }

    public DetailBViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        IBladeService bladeService,
        ILogger<DetailBViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;
        _bladeService = bladeService;

        OpenNewBladeCommand = new AsyncRelayCommand(OpenNewBladeAsync);
    }

    private async Task OpenNewBladeAsync()
    {
        if (SelectedItem is null)
            return;

        var detailsVm = ServiceLocator.Default.GetRequiredService<DetailCViewModel>();
        detailsVm.SetSelectedItem(SelectedItem);
        await _bladeService.OpenBladeAsync(Owner, detailsVm);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        if (!IsInitialized)
        {
            IsInitialized = true;
        }
    }
}

