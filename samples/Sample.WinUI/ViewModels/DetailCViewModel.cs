using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailCViewModel : ViewModelBlade<TestItem>
{
    public DetailCViewModel(
        ICommonServices commonServices,
        ILogger logger,
        TestItem item,
        bool automaticValidation = false)
        : base(commonServices, logger, automaticValidation)
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
