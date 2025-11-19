using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

public class Test2ViewModel(
    ICommonServices commonServices,
    ILogger<Test2ViewModel> logger) : ViewModelDialog<object>(commonServices, logger)
{
}
