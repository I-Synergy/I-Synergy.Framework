using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// Class ErrorPresenter. This class cannot be inherited.
    /// Implements the <see cref="Windows.UI.Xaml.Controls.UserControl" />
    /// Implements the <see cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// Implements the <see cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.UserControl" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
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
