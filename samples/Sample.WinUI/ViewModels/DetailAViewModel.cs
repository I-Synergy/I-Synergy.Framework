using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailAViewModel : ViewModelBlade<TestItem>
{
    public AsyncRelayCommand OpenNewBladeCommand { get; private set; }

    public DetailAViewModel(
        ICommonServices commonServices,
        ILoggerFactory loggerFactory,
        TestItem item,
        bool automaticValidation = false)
        : base(commonServices, loggerFactory, automaticValidation)
    {
        SelectedItem = item;

        OpenNewBladeCommand = new AsyncRelayCommand(OpenNewBladeAsync);
    }

    private async Task OpenNewBladeAsync()
    {
        var detailsVm = new DetailBViewModel(_commonServices, _loggerFactory, SelectedItem);
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
