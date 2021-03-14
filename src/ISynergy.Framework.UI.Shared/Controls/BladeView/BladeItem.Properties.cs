using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// The Blade is used as a child in the BladeView
    /// </summary>
    public partial class BladeItem
    {
        /// <summary>
        /// Identifies the <see cref="TitleBarVisibility" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleBarVisibilityProperty = DependencyProperty.Register(nameof(TitleBarVisibility), typeof(Visibility), typeof(BladeItem), new PropertyMetadata(default(Visibility)));

        /// <summary>
        /// Identifies the <see cref="TitleBarBackground" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleBarBackgroundProperty = DependencyProperty.Register(nameof(TitleBarBackground), typeof(Brush), typeof(BladeItem), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Identifies the <see cref="CloseButtonBackground" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonBackgroundProperty = DependencyProperty.Register(nameof(CloseButtonBackground), typeof(Brush), typeof(BladeItem), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Identifies the <see cref="IsOpen" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(BladeItem), new PropertyMetadata(true, IsOpenChangedCallback));

        /// <summary>
        /// Identifies the <see cref="CloseButtonForeground" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CloseButtonForegroundProperty = DependencyProperty.Register(nameof(CloseButtonForeground), typeof(Brush), typeof(BladeItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the foreground color of the close button
        /// </summary>
        /// <value>The close button foreground.</value>
        public Brush CloseButtonForeground
        {
            get { return (Brush)GetValue(CloseButtonForegroundProperty); }
            set { SetValue(CloseButtonForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visibility of the title bar for this blade
        /// </summary>
        /// <value>The title bar visibility.</value>
        public Visibility TitleBarVisibility
        {
            get { return (Visibility)GetValue(TitleBarVisibilityProperty); }
            set { SetValue(TitleBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background color of the title bar
        /// </summary>
        /// <value>The title bar background.</value>
        public Brush TitleBarBackground
        {
            get { return (Brush)GetValue(TitleBarBackgroundProperty); }
            set { SetValue(TitleBarBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background color of the default close button in the title bar
        /// </summary>
        /// <value>The close button background.</value>
        public Brush CloseButtonBackground
        {
            get { return (Brush)GetValue(CloseButtonBackgroundProperty); }
            set { SetValue(CloseButtonBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this blade is opened
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Determines whether [is open changed callback] [the specified dependency object].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsOpenChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var bladeItem = (BladeItem)dependencyObject;
            bladeItem.Visibility = bladeItem.IsOpen ? Visibility.Visible : Visibility.Collapsed;
            bladeItem.VisibilityChanged?.Invoke(bladeItem, bladeItem.Visibility);
        }
    }
}
