using System.Diagnostics;
using Window = Microsoft.UI.Xaml.Window;

#if WINDOWS10_0_22000_0_OR_GREATER
using System.Runtime.InteropServices;
using WinRT;
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Window extensions class
    /// </summary>
    public static class WindowExtensions
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

#if WINDOWS10_0_22000_0_OR_GREATER
        /// <summary>
        /// IInitializeWithWindow interface.
        /// </summary>
        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            /// <summary>
            /// Initialize with window.
            /// </summary>
            /// <param name="hwnd"></param>
            void Initialize(IntPtr hwnd);
        }

        /// <summary>
        /// IWindowNative interface.
        /// </summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            /// <summary>
            /// Get window handle.
            /// </summary>
            IntPtr WindowHandle { get; }
        }

        /// <summary>
        /// Get handle from window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static IntPtr GetHandle(this Window window)
        {
            return window.As<IWindowNative>().WindowHandle;
        }

        /// <summary>
        /// Initializes window with handle.
        /// </summary>
        /// <param name="window"></param>
        public static void InitializeWindow(this object window)
        {
            var hwnd = GetCurrentProcMainWindowHandle();
            var initializeWithWindow = window.As<IInitializeWithWindow>();
            initializeWithWindow.Initialize(hwnd);
        }
#endif
    }
}
