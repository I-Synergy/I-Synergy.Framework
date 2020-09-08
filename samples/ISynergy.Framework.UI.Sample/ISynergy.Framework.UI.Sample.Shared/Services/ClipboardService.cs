using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Helpers;

namespace ISynergy.Framework.UI.Sample.Services
{
    /// <summary>
    /// Class ClipboardService.
    /// </summary>
    public class ClipboardService : IClipboardService
    {
        /// <summary>
        /// Gets the PNG image from clipboard asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        public Task<byte[]> GetPngImageFromClipboardAsync() =>
            ImageHelper.PngByteArrayFromClipboardImageAsync();
    }
}
