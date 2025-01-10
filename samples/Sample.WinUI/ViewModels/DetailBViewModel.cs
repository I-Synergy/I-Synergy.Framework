using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailBViewModel : ViewModelBlade<TestItem>
{
    public AsyncRelayCommand OpenNewBladeCommand { get; private set; }

    public DetailBViewModel(
        ICommonServices commonServices,
        ILogger logger,
        TestItem item,
        bool automaticValidation = false)
        : base(commonServices, logger, automaticValidation)
    {
        SelectedItem = item;

        OpenNewBladeCommand = new AsyncRelayCommand(OpenNewBladeAsync);
    }

    private async Task OpenNewBladeAsync()
    {
        var detailsVm = new DetailCViewModel(_commonServices, _logger, SelectedItem);
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
