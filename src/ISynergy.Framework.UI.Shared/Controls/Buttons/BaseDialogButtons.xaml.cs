using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class BaseDialogButtons. This class cannot be inherited.
    /// Implements the <see cref="UserControl" />
    /// Implements the <see cref="IComponentConnector" />
    /// Implements the <see cref="IComponentConnector2" />
    /// </summary>
    /// <seealso cref="UserControl" />
    /// <seealso cref="IComponentConnector" />
    /// <seealso cref="IComponentConnector2" />
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
