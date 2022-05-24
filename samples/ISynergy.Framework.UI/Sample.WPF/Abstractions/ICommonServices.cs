using ISynergy.Framework.Clipboard.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace Sample.Abstractions
{
    public interface ICommonServices : IBaseCommonServices
    {
        /// <summary>
        /// Gets the clipboard service.
        /// </summary>
        /// <value>The camera service.</value>
        IClipboardService ClipboardService { get; }
        /// <summary>
        /// Gets the fileservice.
        /// </summary>
        /// <value>The client monitor service.</value>
        IFileService FileService { get; }
    }
}
