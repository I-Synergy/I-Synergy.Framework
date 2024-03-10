using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Views;

namespace Sample.ViewModels;

public class TestViewModel(
    IContext context,
    IBaseCommonServices commonServices,
    ILogger logger,
    bool automaticValidation = false) : ViewModelDialog<object>(context, commonServices, logger, automaticValidation)
{
    public override async Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        Test2ViewModel testVm = new Test2ViewModel(Context, BaseCommonServices, Logger);
        testVm.Submitted += new WeakEventHandler<SubmitEventArgs<object>>(TestVm_Submitted).Handler;
        await BaseCommonServices.DialogService.ShowDialogAsync(typeof(Test2Window), testVm);
    }

    private void TestVm_Submitted(object sender, ISynergy.Framework.Mvvm.Events.SubmitEventArgs<object> e)
    {
        return;
    }
}
