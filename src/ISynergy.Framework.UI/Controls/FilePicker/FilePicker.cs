using ISynergy.Framework.UI.Controls.Abstractions;
using System;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Cross-platform FilePicker implementation
    /// </summary>
    public static class FilePicker
    {
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
