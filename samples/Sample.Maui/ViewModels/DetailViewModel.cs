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
        TestItem item,
        bool automaticValidation = false)
        : base(commonServices, logger)
    {
        SelectedItem = item;
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
