using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailViewModel : ViewModelNavigation<TestItem>
{
    public DetailViewModel(
        ICommonServices commonServices,
        ILogger<DetailViewModel> logger,
        bool automaticValidation = false)
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
