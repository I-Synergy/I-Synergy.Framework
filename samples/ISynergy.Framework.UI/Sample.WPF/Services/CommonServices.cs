using ISynergy.Framework.Clipboard.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Services;
using Sample.Abstractions;

namespace Sample.Services
{
    /// <summary>
    /// Holder for all common services to avoid large sets of constructor parameters.
    /// </summary>
    public class CommonServices : BaseCommonService, ICommonServices
    {
        /// <summary>
        /// Gets the file service.
        /// </summary>
        /// <value>The camera service.</value>
        public IFileService FileService { get; }
        /// <summary>
        /// Gets the camera service.
        /// </summary>
        /// <value>The camera service.</value>
        public IClipboardService ClipboardService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonServices" /> class.
        /// </summary>
        /// <param name="busyService">The busy service.</param>
        /// <param name="messageService"></param>
        /// <param name="languageService">The language service.</param>
        /// <param name="telemetryService">The telemetry service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="infoService">The information service.</param>
        /// <param name="converterService">The converter service.</param>
        /// <param name="dispatcherService"></param>
        /// <param name="fileService"></param>
        /// <param name="clipboardService">The clipboard service.</param>
        public CommonServices(
            IBusyService busyService,
            IMessageService messageService,
            ILanguageService languageService,
            ITelemetryService telemetryService,
            IDialogService dialogService,
            INavigationService navigationService,
            IInfoService infoService,
            IConverterService converterService,
            IDispatcherService dispatcherService,
            IFileService fileService,
            IClipboardService clipboardService)
            : base(busyService, messageService, languageService, telemetryService, dialogService, navigationService, infoService, converterService, dispatcherService)
        {
            FileService = fileService;
            ClipboardService = clipboardService;
        }
    }
}
