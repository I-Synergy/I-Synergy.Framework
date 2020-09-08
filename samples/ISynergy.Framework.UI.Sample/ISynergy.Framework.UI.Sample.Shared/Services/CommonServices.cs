using ISynergy.Framework.Mvvm.Messaging;
using ISynergy.Framework.UI.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Sample.Abstractions.Services;

namespace ISynergy.Framework.UI.Sample.Services
{
    /// <summary>
    /// Class CommonServices.
    /// </summary>
    public class CommonServices : BaseCommonService, ICommonServices
    {
        /// <summary>Gets the authentication service.</summary>
        /// <value>The authentication service.</value>
        public IAuthenticationService AuthenticationService { get; }
        /// <summary>
        /// Gets the master data service.
        /// </summary>
        /// <value>The master data service.</value>
        public IMasterDataService MasterDataService { get; }
        /// <summary>
        /// Gets the settings service.
        /// </summary>
        /// <value>The settings service.</value>
        public ISettingsService SettingsService { get; }
        /// <summary>
        /// Gets the download file service.
        /// </summary>
        /// <value>The download file service.</value>
        public IDownloadFileService DownloadFileService{ get; }
        /// <summary>
        /// Gets the reporting service.
        /// </summary>
        /// <value>The reporting service.</value>
        public IReportingService ReportingService { get; }
        /// <summary>
        /// Gets the printing service.
        /// </summary>
        /// <value>The printing service.</value>
        public IPrintingService PrintingService { get; }
        /// <summary>
        /// Gets the file service.
        /// </summary>
        /// <value>The file service.</value>
        public IFileService FileService { get; }
        /// <summary>
        /// Gets the camera service.
        /// </summary>
        /// <value>The camera service.</value>
        public ICameraService CameraService { get; }
        /// <summary>
        /// Gets the clipboard service.
        /// </summary>
        /// <value>The clipboard service.</value>
        public IClipboardService ClipboardService { get; }
        /// <summary>
        /// The client monitor service
        /// </summary>
        public IClientMonitorService ClientMonitorService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonServices"/> class.
        /// </summary>
        /// <param name="messenger">The messenger.</param>
        /// <param name="busyService">The busy service.</param>
        /// <param name="languageService">The language service.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="applicationSettingsService">The application settings service.</param>
        /// <param name="telemetryService">The telemetry service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="fileService">The file service.</param>
        /// <param name="cameraService">The camera service.</param>
        /// <param name="infoService">The information service.</param>
        /// <param name="converterService">The converter service.</param>
        /// <param name="masterDataService">The master data service.</param>
        /// <param name="downloadFileService">The download file service.</param>
        /// <param name="reportingService">The reporting service.</param>
        /// <param name="printingService">The printing service.</param>
        /// <param name="clipboardService">The clipboard service.</param>
        public CommonServices(
            IMessenger messenger,
            IBusyService busyService,
            ILanguageService languageService,
            ISettingsService settingsService,
            IApplicationSettingsService applicationSettingsService,
            ITelemetryService telemetryService,
            IDialogService dialogService,
            IUIVisualizerService uiVisualizerService,
            INavigationService navigationService,
            IFileService fileService,
            ICameraService cameraService,
            IInfoService infoService,
            IConverterService converterService,
            IAuthenticationService authenticationService,
            IMasterDataService masterDataService,
            IDownloadFileService downloadFileService,
            IReportingService reportingService,
            IPrintingService printingService,
            IClipboardService clipboardService,
            IClientMonitorService clientMonitorService)
            :base(messenger, busyService, languageService, applicationSettingsService, telemetryService, dialogService, uiVisualizerService, navigationService, infoService, converterService)
        {
            AuthenticationService = authenticationService;
            MasterDataService = masterDataService;
            SettingsService = settingsService;
            DownloadFileService = downloadFileService;
            ReportingService = reportingService;
            PrintingService = printingService;
            FileService = fileService;
            CameraService = cameraService;
            ClipboardService = clipboardService;
            ClientMonitorService = clientMonitorService;
        }
    }
}
