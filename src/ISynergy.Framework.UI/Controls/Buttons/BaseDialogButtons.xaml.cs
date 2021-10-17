#if (WINDOWS_UWP || HAS_UNO)
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class BaseDialogButtons. This class cannot be inherited.
    /// Implements the <see cref="UserControl" />
    /// Implements the <see cref="IComponentConnector" />
    /// </summary>
    /// <seealso cref="UserControl" />
    /// <seealso cref="IComponentConnector" />
    public sealed partial class BaseDialogButtons : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDialogButtons"/> class.
        /// </summary>
        public BaseDialogButtons()
        {
            InitializeComponent();
        }
    }
}
