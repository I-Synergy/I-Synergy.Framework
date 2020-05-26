using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// Defines a control for providing a header for read-only text.
    /// </summary>
    [TemplatePart(Name = "HeaderContentPresenter", Type = typeof(ContentPresenter))]
    [ContentProperty(Name = nameof(Inlines))]
    public partial class HeaderedTextBlock : Control
    {
        /// <summary>
        /// The header content presenter
        /// </summary>
        private ContentPresenter _headerContentPresenter;
        /// <summary>
        /// The text content
        /// </summary>
        private TextBlock _textContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedTextBlock" /> class.
        /// </summary>
        public HeaderedTextBlock()
        {
            DefaultStyleKey = typeof(HeaderedTextBlock);
        }

        /// <summary>
        /// Called when applying the control template.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _headerContentPresenter = GetTemplateChild("HeaderContentPresenter") as ContentPresenter;
            _textContent = GetTemplateChild("TextContent") as TextBlock;

            UpdateVisibility();
            Inlines.AddItemsToTextBlock(_textContent);
            UpdateForOrientation(Orientation);
        }

        /// <summary>
        /// Updates the visibility.
        /// </summary>
        private void UpdateVisibility()
        {
            if (_headerContentPresenter != null)
            {
                _headerContentPresenter.Visibility = _headerContentPresenter.Content == null
                                                     ? Visibility.Collapsed
                                                     : Visibility.Visible;
            }

            if (_textContent != null)
            {
                _textContent.Visibility = string.IsNullOrWhiteSpace(_textContent.Text) && HideTextIfEmpty
                                                    ? Visibility.Collapsed
                                                    : Visibility.Visible;
            }
        }

        /// <summary>
        /// Updates for orientation.
        /// </summary>
        /// <param name="orientationValue">The orientation value.</param>
        private void UpdateForOrientation(Orientation orientationValue)
        {
            switch (orientationValue)
            {
                case Orientation.Vertical:
                    VisualStateManager.GoToState(this, "Vertical", true);
                    break;
                case Orientation.Horizontal:
                    VisualStateManager.GoToState(this, "Horizontal", true);
                    break;
            }
        }
    }
}
