using ISynergy.Framework.Core.Enumerations;
using System.Threading.Tasks;

namespace ISynergy.Framework.Clipboard.Abstractions.Services
{
    /// <summary>
    /// Interface IClipboardService
    /// </summary>
    public interface IClipboardService
    {
        /// <summary>
        /// Gets the bitmap source from clipboard asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        Task<object> GetBitmapSourceFromClipboardAsync();

        /// <summary>
        /// Gets the byte array from clipboard image asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        Task<byte[]> GetByteArrayFromClipboardImageAsync(ImageFormats format);
    }
}
