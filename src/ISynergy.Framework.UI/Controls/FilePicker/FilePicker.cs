using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.Ui.Controls.Abstractions;
using System;
using System.Collections.Generic;

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
        public static readonly List<FileType> FileTypes = new List<FileType>
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
        };

        /// <summary>
        /// Lazy-initialized file picker implementation
        /// </summary>
        private static Lazy<IFilePicker> implementation = new Lazy<IFilePicker>(CreateFilePicker, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current file picker plugin implementation to use
        /// </summary>
        public static IFilePicker Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
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

        /// <summary>
        /// Returns new exception to throw when implementation is not found. This is the case when
        /// the NuGet package is not added to the platform specific project.
        /// </summary>
        /// <returns>exception to throw</returns>
        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException(
                "This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
}
