using ISynergy.Framework.Core.Models;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Core.Constants
{
    public static class MasterData
    {
        /// <summary>
        /// Gets the file types.
        /// </summary>
        /// <value>The file types.</value>
        /// #if __ANDROID__
        /// var filters = FilePicker.FileTypes.Where(q =&gt; q.IsImage).Select(s =&gt; s.ContentType).ToList();
        /// #else
        /// var filters = FilePicker.FileTypes.Where(q =&gt; q.IsImage).Select(s =&gt; s.Extension).ToList();
        /// #endif
        public readonly static ReadOnlyCollection<FileType> FileTypes = new List<FileType>
        {
            new FileType(0, "Textfile", ".txt", false, "text/plain"),
            new FileType(1, "Microsoft Word (*.doc)", ".doc", false, "application/msword"),
            new FileType(2, "Microsoft Word (*.docx)", ".docx", false, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
            new FileType(3, "Microsoft Excel (*.xls)", ".xls", false, "application/vnd.ms-excel"),
            new FileType(4, "Microsoft Excel (*.xlsx)", ".xlsx", false, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
            new FileType(5, "Adobe PDF", ".pdf", false, "application/pdf"),
            new FileType(6, "JPEG Image", ".jpg", true, "image/jpeg"),
            new FileType(7, "JPEG Image", ".jpeg", true, "image/jpeg"),
            new FileType(8, "PNG Image", ".png", true, "image/png"),
            new FileType(9, "GIF Image", ".gif", true, "image/gif")
        }.AsReadOnly();
    }
}
