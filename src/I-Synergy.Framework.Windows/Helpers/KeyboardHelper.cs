using Windows.System;
using Windows.UI.Core;

namespace ISynergy.Framework.Windows.Controls.Helpers
{
    /// <summary>
    /// Class KeyboardHelper.
    /// </summary>
    internal static class KeyboardHelper
    {
        /// <summary>
        /// Determines whether [is modifier key down] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if [is modifier key down] [the specified key]; otherwise, <c>false</c>.</returns>
        public static bool IsModifierKeyDown(VirtualKey key)
        {
            var window = CoreWindow.GetForCurrentThread();
            if (window is null)
            {
                return false;
            }

            var state = window.GetKeyState(key);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
    }
}
