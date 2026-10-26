using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

/// <summary>
/// Second, nested blade of <see cref="DetailsViewModel"/>. It opens no further blade and is
/// closed through the inherited cancel/close command.
/// </summary>
public class DetailAViewModel : ViewModelBlade<TestItem>
{
    public DetailAViewModel(
        ICommonServices commonServices,
        ILogger<DetailAViewModel> logger)
        : base(commonServices, logger)
    {
        Title = "Detail A";
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
