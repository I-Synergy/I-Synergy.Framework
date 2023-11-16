using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.Logging;

namespace Sample.Services
{
    public class ExceptionHandlerService : BaseExceptionHandlerService
    {
        public ExceptionHandlerService(
            IBusyService busyService, 
            IDialogService dialogService, 
            ILanguageService languageService, 
            ILogger<BaseExceptionHandlerService> logger) 
            : base(busyService, dialogService, languageService, logger)
        {
        }
    }
}
