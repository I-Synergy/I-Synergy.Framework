using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

/// <summary>
/// First blade of <see cref="DetailsViewModel"/>. Opens a nested blade on demand.
/// </summary>
public class DetailViewModel : ViewModelBlade<TestItem>
{
    private readonly IBladeService _bladeService;

    public AsyncRelayCommand OpenNewBladeCommand { get; private set; }

    public DetailViewModel(
        ICommonServices commonServices,
        IBladeService bladeService,
        ILogger<DetailViewModel> logger)
        : base(commonServices, logger)
    {
        _bladeService = bladeService;

        Title = "Detail";

        OpenNewBladeCommand = new AsyncRelayCommand(OpenNewBladeAsync);
    }

    private async Task OpenNewBladeAsync()
    {
        if (SelectedItem is null)
            return;

        await _bladeService.OpenBladeAsync<DetailAViewModel, TestItem>(Owner, SelectedItem);
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
