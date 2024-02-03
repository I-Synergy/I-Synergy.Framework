using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Services.Base;

namespace Sample.Services;

public class ExceptionHandlerService(
    IBusyService busyService,
    IDialogService dialogService,
    ILanguageService languageService,
    ILogger<BaseExceptionHandlerService> logger) : BaseExceptionHandlerService(busyService, dialogService, languageService, logger)
{
}
