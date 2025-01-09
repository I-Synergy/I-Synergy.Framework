using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

public class TestExceptionViewModel : ViewModelDialog<object>
{
    public TestExceptionViewModel(
        IScopedContextService scopedContextService,
        ICommonServices commonServices,
        ILogger logger,
        bool automaticValidation = false)
        : base(scopedContextService, commonServices, logger, automaticValidation)
    {
    }

    public override Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        throw new Exception("This is a test exception");
    }
}
