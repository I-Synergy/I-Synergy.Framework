using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
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
        await dialogService.ShowDialogAsync<Test2Window, Test2ViewModel, object>(
            _ => Task.CompletedTask);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}

