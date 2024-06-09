using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

public class TestExceptionViewModel : ViewModelDialog<object>
{
    public TestExceptionViewModel(
        IContext context, 
        IBaseCommonServices commonServices, 
        ILogger logger, 
        bool automaticValidation = false) 
        : base(context, commonServices, logger, automaticValidation)
    {
    }

    public override async Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        throw new Exception("This is a test exception");
        await base.SubmitAsync(e, validateUnderlayingProperties);
    }
}
