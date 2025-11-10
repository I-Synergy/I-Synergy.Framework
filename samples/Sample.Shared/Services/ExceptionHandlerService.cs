using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.Logging;

namespace Sample.Services;

public class ExceptionHandlerService(
    IBusyService busyService,
    ILanguageService languageService,
    IMessengerService messengerService,
    ILogger<BaseExceptionHandlerService> logger) : BaseExceptionHandlerService(busyService, languageService, messengerService, logger)
{
}