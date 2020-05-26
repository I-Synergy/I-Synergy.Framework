using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// Class BaseMenu. This class cannot be inherited.
    /// Implements the <see cref="Windows.UI.Xaml.Controls.CommandBar" />
    /// Implements the <see cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// Implements the <see cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.CommandBar" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class BaseMenu
    {
        /// <summary>
        /// The refresh enabled property
        /// </summary>
        private static readonly DependencyProperty Refresh_EnabledProperty =
            DependencyProperty.Register(nameof(Refresh_Enabled), typeof(Visibility), typeof(BaseMenu), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the refresh enabled.
        /// </summary>
        /// <value>The refresh enabled.</value>
        public Visibility Refresh_Enabled
        {
            get { return (Visibility)GetValue(Refresh_EnabledProperty); }
            set { SetValue(Refresh_EnabledProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMenu"/> class.
        /// </summary>
        public BaseMenu()
        {
            InitializeComponent();
        }
    }
}
