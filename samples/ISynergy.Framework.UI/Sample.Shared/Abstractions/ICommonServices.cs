using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace Sample.Abstractions.Services
{
    /// <summary>
    /// Interface ICommonServices
    /// Implements the <see cref="IBaseCommonServices" />
    /// </summary>
    /// <seealso cref="IBaseCommonServices" />
    public interface ICommonServices : IBaseCommonServices
    {
        /// <summary>
        /// Gets the authentication service.
        /// </summary>
        /// <value>The authentication service.</value>
        IAuthenticationService AuthenticationService { get; }
        /// <summary>
        /// Gets the master data service.
        /// </summary>
        /// <value>The master data service.</value>
        IMasterDataService MasterDataService { get; }
        /// <summary>
        /// Gets the download file service.
        /// </summary>
        /// <value>The download file service.</value>
        IDownloadFileService DownloadFileService { get; }
        /// <summary>
        /// Gets the reporting service.
        /// </summary>
        /// <value>The reporting service.</value>
        IReportingService ReportingService { get; }
        /// <summary>
        /// Gets the printing service.
        /// </summary>
        /// <value>The printing service.</value>
        IPrintingService PrintingService { get; }
        /// <summary>
        /// Gets the file service.
        /// </summary>
        /// <value>The file service.</value>
        IFileService FileService { get; }
        /// <summary>
        /// Gets the camera service.
        /// </summary>
        /// <value>The camera service.</value>
        ICameraService CameraService { get; }
        /// <summary>
        /// Gets the clipboard service.
        /// </summary>
        /// <value>The clipboard service.</value>
        IClipboardService ClipboardService { get; }
        /// <summary>
        /// Gets the client monitor service.
        /// </summary>
        /// <value>The client monitor service.</value>
        IClientMonitorService ClientMonitorService { get; }
    }
}
