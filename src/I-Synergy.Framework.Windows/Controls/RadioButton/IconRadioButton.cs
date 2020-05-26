using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// Class IconRadioButton.
    /// Implements the <see cref="Windows.UI.Xaml.Controls.RadioButton" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.RadioButton" />
    public class IconRadioButton : RadioButton
    {
        /// <summary>
        /// The path icon property
        /// </summary>
        public static readonly DependencyProperty PathIconProperty =
            DependencyProperty.Register(nameof(PathIcon), typeof(string), typeof(IconRadioButton), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the path icon.
        /// </summary>
        /// <value>The path icon.</value>
        public string PathIcon
        {
            get => (string)GetValue(PathIconProperty);
            set => SetValue(PathIconProperty, value);
        }
    }
}
