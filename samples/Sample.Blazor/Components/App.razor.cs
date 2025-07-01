using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI;

namespace Sample.Components;

public partial class App : Application
{
    public App(
        ICommonServices commonServices,
        ISettingsService settingsService,
        IExceptionHandlerService exceptionHandlerService,
        ILogger<Application> logger)
        : base(commonServices, settingsService, exceptionHandlerService, logger)
    {
    }

    protected override Task InitializeApplicationAsync()
    {
        return Task.CompletedTask;
    }

    protected override void OnAuthenticationChanged(object? sender, ReturnEventArgs<bool> e)
    {
    }
}
