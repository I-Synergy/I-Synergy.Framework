using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class ErrorPresenter. This class cannot be inherited.
    /// Implements the <see cref="UserControl" />
    /// Implements the <see cref="IComponentConnector" />
    /// Implements the <see cref="IComponentConnector2" />
    /// </summary>
    /// <seealso cref="UserControl" />
    /// <seealso cref="IComponentConnector" />
    /// <seealso cref="IComponentConnector2" />
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
