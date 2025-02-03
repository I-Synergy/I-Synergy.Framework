using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;

namespace Sample.ViewModels;

public class TestExceptionViewModel : ViewModelDialog<object>
{
    public TestExceptionViewModel(
        ICommonServices commonServices,
        bool automaticValidation = false)
        : base(commonServices, automaticValidation)
    {
    }

    public override Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        throw new Exception("This is a test exception");
    }
}
