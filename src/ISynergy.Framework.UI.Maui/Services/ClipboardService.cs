using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ClipboardService.
    /// </summary>
    public class ClipboardService : IClipboardService
    {
        /// <summary>
        /// Gets the bitmap source from clipboard asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> GetBitmapSourceFromClipboardAsync()
        {
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Gets the byte array from clipboard image asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        public Task<byte[]> GetByteArrayFromClipboardImageAsync(ImageFormats format)
        {
            byte[] result = null;
            return Task.FromResult(result);
        }
    }
}
