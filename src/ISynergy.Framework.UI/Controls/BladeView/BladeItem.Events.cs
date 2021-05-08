using System;

#if (__UWP__ || HAS_UNO)
using Windows.UI.Xaml;
#elif (__WINUI__)
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// The Blade is used as a child in the BladeView
    /// </summary>
    public partial class BladeItem
    {
        /// <summary>
        /// Fires when the blade is opened or closed
        /// </summary>
        public event EventHandler<Visibility> VisibilityChanged;
    }
}
