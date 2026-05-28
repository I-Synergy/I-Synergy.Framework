using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class TestExceptionViewModel : ViewModelDialog<TestItem>
{
    public TestExceptionViewModel(
        ICommonServices commonServices,
        ILogger<TestExceptionViewModel> logger)
        : base(commonServices, logger)
    {
        SelectedItem = new TestItem { Description = "Test Item" };
        Title = SelectedItem.Description;
    }

    public override Task SubmitAsync(TestItem e, bool validateUnderlayingProperties = true)
    {
        throw new InvalidOperationException("This is a test exception");
    }
}
