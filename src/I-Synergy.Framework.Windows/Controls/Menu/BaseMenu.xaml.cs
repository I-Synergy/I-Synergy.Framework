using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ISynergy.Framework.Windows.Controls
{
    public sealed partial class BaseMenu
    {
        private static readonly DependencyProperty Refresh_EnabledProperty =
            DependencyProperty.Register(nameof(Refresh_Enabled), typeof(Visibility), typeof(BaseMenu), new PropertyMetadata(Visibility.Visible));

        public Visibility Refresh_Enabled
        {
            get { return (Visibility)GetValue(Refresh_EnabledProperty); }
            set { SetValue(Refresh_EnabledProperty, value); }
        }

        public BaseMenu()
        {
            InitializeComponent();
        }
    }
}
