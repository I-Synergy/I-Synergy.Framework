using System;
using System.Diagnostics;

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Window extensions class
    /// </summary>
    public static partial class WindowExtensions
    {
        /// <summary>
        /// Get handle from main window.
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetCurrentProcMainWindowHandle()
        {
            using var process = Process.GetCurrentProcess();
            return process.MainWindowHandle;
        }
    }
}
