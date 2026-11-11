using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public partial class DetailBViewModel : ViewModelBlade<TestItem>
{
    private readonly IBladeService _bladeService;

    public AsyncRelayCommand OpenNewBladeCommand { get; private set; }

    public DetailBViewModel(
        ICommonServices commonServices,
        IBladeService bladeService,
        ILogger<DetailBViewModel> logger)
        : base(commonServices, logger)
    {
        _bladeService = bladeService;

        OpenNewBladeCommand = new AsyncRelayCommand(OpenNewBladeAsync);
    }

    private async Task OpenNewBladeAsync()
    {
        if (SelectedItem is null)
            return;

        await _bladeService.OpenBladeAsync<DetailCViewModel, TestItem>(Owner, SelectedItem);
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

