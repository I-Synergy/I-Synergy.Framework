using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using NugetUnlister.Common.Abstractions;
using ISynergy.Framework.UI.Services;

namespace NugetUnlister.Services
{
    internal class CommonServices : BaseCommonService, ICommonServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonServices" /> class.
        /// </summary>
        /// <param name="busyService">The busy indicator service.</param>
        /// <param name="messageService">The messaging service</param>
        /// <param name="languageService">The language service.</param>
        /// <param name="telemetryService">The telemetry service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="fileService">The file service.</param>
        /// <param name="infoService">The information service.</param>
        /// <param name="converterService">The converter service.</param>
        /// <param name="dispatcherService">The dispatcher service.</param>
        public CommonServices(
            IBusyService busyService,
            IMessageService messageService,
            ILanguageService languageService,
            ITelemetryService telemetryService,
            IDialogService dialogService,
            INavigationService navigationService,
            IFileService fileService,
            IInfoService infoService,
            IConverterService converterService,
            IDispatcherService dispatcherService)
            : base(busyService,
                 messageService,
                 languageService,
                 telemetryService,
                 dialogService,
                 navigationService,
                 infoService,
                 converterService,
                 dispatcherService)
        {
        }
    }
}
