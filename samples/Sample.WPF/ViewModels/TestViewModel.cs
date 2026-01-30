using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Views;

namespace Sample.ViewModels;

public class TestViewModel(
    ICommonServices commonServices,
    IDialogService dialogService,
    ILogger<TestViewModel> logger) : ViewModelDialog<object>(commonServices, logger)
{
    public override async Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        var testVm = _commonServices.ScopedContextService.GetRequiredService<Test2ViewModel>();
        testVm.Submitted += TestVm_Submitted;
        await dialogService.ShowDialogAsync(typeof(Test2Window), testVm);
    }

    private void TestVm_Submitted(object? sender, SubmitEventArgs<object> e)
    {
        if (sender is Test2ViewModel vm)
            vm.Submitted -= TestVm_Submitted;

        return;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
