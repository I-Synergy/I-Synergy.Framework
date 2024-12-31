using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.Logging;

namespace Sample.Services;

public class ExceptionHandlerService : BaseExceptionHandlerService
{
    public ExceptionHandlerService(
        IBusyService busyService,
        IDialogService dialogService,
        ILogger<BaseExceptionHandlerService> logger)
        : base(busyService, dialogService, logger)
    {
        _logger.LogDebug($"ExceptionHandlerService instance created with ID: {Guid.NewGuid()}");
    }
}