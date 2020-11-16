using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.Ui.Controls.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Ui.Controls
{
    /// <summary>
    /// Cross-platform FilePicker implementation
    /// </summary>
    public static class FilePicker
    {
        /// <summary>
        /// Gets the file types.
        /// </summary>
        /// <value>The file types.</value>
        public static readonly ReadOnlyCollection<FileType> FileTypes = new List<FileType>
        {
            new FileType(0, "Textfile", ".txt", false, "text/plain"),
            new FileType(1, "Microsoft Word (*.doc)", ".doc", false, "application/msword"),
            new FileType(2, "Microsoft Word (*.docx)", ".docx", false, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
            new FileType(3, "Microsoft Excel (*.xls)", ".xls", false, "application/vnd.ms-excel"),
            new FileType(4, "Microsoft Excel (*.xlsx)", ".xlsx", false, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
            new FileType(5, "Adobe PDF", ".pdf", false, "application/pdf"),
            new FileType(6, "JPEG Image", ".jpg", true, "image/jpeg"),
            new FileType(7, "PNG Image", ".png", true, "image/png"),
            new FileType(8, "GIF Image", ".gif", true, "image/gif")
        }.AsReadOnly();

        /// <summary>
        /// Lazy-initialized file picker implementation
        /// </summary>
        private static readonly Lazy<IFilePicker> implementation = new Lazy<IFilePicker>(CreateFilePicker, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current file picker plugin implementation to use
        /// </summary>
        public static IFilePicker Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret is null)
                {
                    throw new NotImplementedException(
                        "This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }

                return ret;
            }
        }

        /// <summary>
        /// Creates file picker instance for the platform
        /// </summary>
        /// <returns>file picker instance</returns>
        private static IFilePicker CreateFilePicker()
        {
#if NETSTANDARD1_0 || NETSTANDARD2_0
            return null;
#else
            return new FilePickerImplementation();
#endif
        }
    }
}
