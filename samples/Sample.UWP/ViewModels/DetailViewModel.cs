using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailViewModel : ViewModelBlade<TestItem>
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    public AsyncRelayCommand OpenNewBladeCommand { get; private set; }

    public DetailViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ILogger<DetailViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;

        OpenNewBladeCommand = new AsyncRelayCommand(OpenNewBladeAsync);
    }

    private async Task OpenNewBladeAsync()
    {
        var detailsVm = _commonServices.ScopedContextService.GetRequiredService<DetailAViewModel>();
        detailsVm.SetSelectedItem(SelectedItem);
        await _navigationService.OpenBladeAsync(Owner, detailsVm);
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
