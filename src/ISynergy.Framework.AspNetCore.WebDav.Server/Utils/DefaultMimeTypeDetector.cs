using System.IO;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Utils
{
    /// <summary>
    /// The default implementation of a <see cref="IMimeTypeDetector"/>
    /// </summary>
    public class DefaultMimeTypeDetector : IMimeTypeDetector
    {
        /// <inheritdoc />
        public bool TryDetect(IEntry entry, out string mimeType)
        {
            var fileExt = Path.GetExtension(entry.Name);
            if (string.IsNullOrEmpty(fileExt))
            {
                mimeType = Utils.MimeTypesMap.DefaultMimeType;
                return false;
            }

            return MimeTypesMap.TryGetMimeType(fileExt.Substring(1), out mimeType);
        }
    }
}
