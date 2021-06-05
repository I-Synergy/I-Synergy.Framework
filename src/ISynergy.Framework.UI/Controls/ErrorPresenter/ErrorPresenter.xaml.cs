#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class ErrorPresenter. This class cannot be inherited.
    /// Implements the <see cref="UserControl" />
    /// Implements the <see cref="IComponentConnector" />
    /// </summary>
    /// <seealso cref="UserControl" />
    /// <seealso cref="IComponentConnector" />
    public sealed partial class ErrorPresenter : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorPresenter"/> class.
        /// </summary>
        public ErrorPresenter()
        {
            InitializeComponent();
        }
    }
}
