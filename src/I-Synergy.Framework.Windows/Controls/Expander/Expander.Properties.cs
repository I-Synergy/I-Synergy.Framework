using Windows.UI.Xaml;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// The <see cref="Expander" /> control allows user to show/hide content based on a boolean state
    /// </summary>
    public partial class Expander
    {
        /// <summary>
        /// Identifies the <see cref="IsExpanded" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(Expander), new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ExpandDirection" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(nameof(ExpandDirection), typeof(ExpandDirection), typeof(Expander), new PropertyMetadata(ExpandDirection.Down, OnExpandDirectionChanged));

        /// <summary>
        /// Identifies the <see cref="ContentOverlay" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentOverlayProperty =
            DependencyProperty.Register(nameof(ContentOverlay), typeof(UIElement), typeof(Expander), new PropertyMetadata(default(UIElement)));

        /// <summary>
        /// Identifies the <see cref="HeaderStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(nameof(HeaderStyle), typeof(Style), typeof(Expander), new PropertyMetadata(default(Style)));

        /// <summary>
        /// Gets or sets a value indicating whether the content of the control is opened/visible or closed/hidden.
        /// </summary>
        /// <value><c>true</c> if this instance is expanded; otherwise, <c>false</c>.</value>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Expand Direction of the control.
        /// </summary>
        /// <value>The expand direction.</value>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ContentOverlay of the control.
        /// </summary>
        /// <value>The content overlay.</value>
        public UIElement ContentOverlay
        {
            get { return (UIElement)GetValue(ContentOverlayProperty); }
            set { SetValue(ContentOverlayProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value for the style to use for the Header of the Expander.
        /// </summary>
        /// <value>The header style.</value>
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        /// <summary>
        /// Handles the <see cref="E:IsExpandedPropertyChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expander = d as Expander;

            var isExpanded = (bool)e.NewValue;
            if (isExpanded)
            {
                expander.ExpandControl();
            }
            else
            {
                expander.CollapseControl();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ExpandDirectionChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnExpandDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expander = d as Expander;
            var previousExpandDirection = (ExpandDirection)e.OldValue;
            var newExpandDirection = (ExpandDirection)e.NewValue;

            if (previousExpandDirection != newExpandDirection)
            {
                expander.OnExpandDirectionChanged();
            }
        }
    }
}
