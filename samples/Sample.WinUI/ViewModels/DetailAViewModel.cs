using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailAViewModel : ViewModelBlade<TestItem>
{
    public AsyncRelayCommand OpenNewBladeCommand { get; }

    public DetailAViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger,
        TestItem item,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        SelectedItem = item;

        OpenNewBladeCommand = new AsyncRelayCommand(OpenNewBladeAsync);
    }

    private async Task OpenNewBladeAsync()
    {
        var detailsVm = new DetailBViewModel(Context, BaseCommonServices, Logger, SelectedItem);
        await BaseCommonServices.NavigationService.OpenBladeAsync(Owner, detailsVm);
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
