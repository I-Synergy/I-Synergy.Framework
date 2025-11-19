using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailCViewModel : ViewModelBlade<TestItem>
{
    public DetailCViewModel(
        ICommonServices commonServices,
        ILogger<DetailCViewModel> logger)
        : base(commonServices, logger)
    {
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
