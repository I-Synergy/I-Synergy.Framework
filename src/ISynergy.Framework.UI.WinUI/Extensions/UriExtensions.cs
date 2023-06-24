using ISynergy.Framework.Core.Extensions;
using Windows.Storage;

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Class UriExtensins.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// convert image source to byte array as an asynchronous operation.
        /// </summary>
        /// <param name="_self">The image source.</param>
        /// <returns>System.Byte[].</returns>
        public static async Task<byte[]> ToByteArrayAsync(this Uri _self)
        {
            byte[] result = null;

            var file = await StorageFile.GetFileFromApplicationUriAsync(_self);

            using (var inputStream = await file.OpenStreamForReadAsync().ConfigureAwait(false))
            {
                result = await inputStream.ToByteArrayAsync().ConfigureAwait(false);
            }

            return result;
        }
    }
}