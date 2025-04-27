using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

public class TestExceptionViewModel : ViewModelDialog<object>
{
    public TestExceptionViewModel(
        ICommonServices commonServices,
        ILogger<TestExceptionViewModel> logger)
        : base(commonServices, logger)
    {
    }

    public override Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        throw new Exception("This is a test exception");
    }
}
