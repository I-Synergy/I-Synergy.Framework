using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Sample.Models;

namespace Sample.ViewModels;

public class SampleDialogViewModel : ViewModelDialog<Budget>
{
    public SampleDialogViewModel(ICommonServices commonServices, ILogger<ViewModelDialog<Budget>> logger) : base(commonServices, logger)
    {
    }
}
