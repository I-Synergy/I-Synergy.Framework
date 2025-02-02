using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Sample.Views;

namespace Sample.ViewModels;

public class TestViewModel(
    ICommonServices commonServices,
    bool automaticValidation = false) : ViewModelDialog<object>(commonServices, automaticValidation)
{
    public override async Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        Test2ViewModel testVm = new Test2ViewModel(_commonServices);
        testVm.Submitted += new WeakEventHandler<SubmitEventArgs<object>>(TestVm_Submitted).Handler;
        await _commonServices.DialogService.ShowDialogAsync(typeof(Test2Window), testVm);
    }

    private void TestVm_Submitted(object sender, ISynergy.Framework.Mvvm.Events.SubmitEventArgs<object> e)
    {
        if (sender is Test2ViewModel vm)
            vm.Submitted -= TestVm_Submitted;

        return;
    }
}
