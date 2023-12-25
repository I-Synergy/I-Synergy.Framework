using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

public class Test2ViewModel(
    IContext context,
    IBaseCommonServices commonServices,
    ILogger logger,
    bool automaticValidation = false) : ViewModelDialog<object>(context, commonServices, logger, automaticValidation)
{
}
