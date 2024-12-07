using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class DetailViewModel : ViewModelNavigation<TestItem>
{
    public DetailViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger,
        TestItem item,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
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
