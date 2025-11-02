using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.ViewModels;

[Lifetime(Lifetimes.Singleton)]
public sealed class LoadingViewModel : ViewModel
{
    public LoadingViewModel(ICommonServices commonServices, ILogger<LoadingViewModel> logger)
        : base(commonServices, logger)
    {
    }
}
