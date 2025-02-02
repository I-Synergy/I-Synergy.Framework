using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

public class Test2ViewModel(
    ICommonServices commonServices,
    ILoggerFactory loggerFactory,
    bool automaticValidation = false) : ViewModelDialog<object>(commonServices, loggerFactory, automaticValidation)
{
}
