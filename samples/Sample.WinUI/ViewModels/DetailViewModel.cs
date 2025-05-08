using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailViewModel : ViewModelBlade<TestItem>
{
    public AsyncRelayCommand OpenNewBladeCommand { get; private set; }

    public DetailViewModel(
        ICommonServices commonServices,
        ILogger<DetailViewModel> logger)
        : base(commonServices, logger)
    {
        OpenNewBladeCommand = new AsyncRelayCommand(OpenNewBladeAsync);
    }

    private async Task OpenNewBladeAsync()
    {
        var detailsVm = _commonServices.ScopedContextService.GetRequiredService<DetailAViewModel>();
        detailsVm.SetSelectedItem(SelectedItem);
        await _commonServices.NavigationService.OpenBladeAsync(Owner, detailsVm);
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
