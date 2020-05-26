using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// Class BaseDialogButtons. This class cannot be inherited.
    /// Implements the <see cref="Windows.UI.Xaml.Controls.UserControl" />
    /// Implements the <see cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// Implements the <see cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.UserControl" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
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
