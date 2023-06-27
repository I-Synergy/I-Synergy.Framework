using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Views;

namespace Sample.ViewModels
{
    public class TestViewModel : ViewModelDialog<object>
    {
        public TestViewModel(
            IContext context, 
            IBaseCommonServices commonServices, 
            ILogger logger, 
            bool automaticValidation = false) 
            : base(context, commonServices, logger, automaticValidation)
        {
        }

        public override async Task SubmitAsync(object e)
        {
            var testVm = new Test2ViewModel(Context, BaseCommonServices, Logger);
            testVm.Submitted += TestVm_Submitted;
            await BaseCommonServices.DialogService.ShowDialogAsync(typeof(Test2Window), testVm);
        }

        private void TestVm_Submitted(object sender, ISynergy.Framework.Mvvm.Events.SubmitEventArgs<object> e)
        {
            if (sender is Test2ViewModel vm)
                vm.Submitted -= TestVm_Submitted;
        }
    }
}
