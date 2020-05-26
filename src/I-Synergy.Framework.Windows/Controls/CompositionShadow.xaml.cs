using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// The <see cref="CompositionShadow" /> control allows the creation of a DropShadow for any Xaml FrameworkElement in markup
    /// making it easier to add shadows to Xaml without having to directly drop down to Windows.UI.Composition APIs.
    /// </summary>
    [ContentProperty(Name = nameof(CastingElement))]
    public sealed partial class CompositionShadow : UserControl
    {
        /// <summary>
        /// The blur radius property
        /// </summary>
        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(CompositionShadow), new PropertyMetadata(9.0, OnBlurRadiusChanged));

        /// <summary>
        /// The color property
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Color), typeof(CompositionShadow), new PropertyMetadata(Colors.Black, OnColorChanged));

        /// <summary>
        /// The offset x property
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetXChanged));

        /// <summary>
        /// The offset y property
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetYChanged));

        /// <summary>
        /// The offset z property
        /// </summary>
        public static readonly DependencyProperty OffsetZProperty =
            DependencyProperty.Register(nameof(OffsetZ), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetZChanged));

        /// <summary>
        /// The shadow opacity property
        /// </summary>
        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(CompositionShadow), new PropertyMetadata(1.0, OnShadowOpacityChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionShadow"/> class.
        /// </summary>
        public CompositionShadow()
        {
            InitializeComponent();
            DefaultStyleKey = typeof(CompositionShadow);
            SizeChanged += CompositionShadow_SizeChanged;
            Loaded += CompositionShadow_Loaded;
            Unloaded += CompositionShadow_Unloaded;

            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _shadowVisual = compositor.CreateSpriteVisual();
            _dropShadow = compositor.CreateDropShadow();
            _shadowVisual.Shadow = _dropShadow;

            // SetElementChildVisual on the control itself ("this") would result in the shadow
            // rendering on top of the content. To avoid this, CompositionShadow contains a Border
            // (to host the shadow) and a ContentPresenter (to hose the actual content, "CastingElement").
            ElementCompositionPreview.SetElementChildVisual(ShadowElement, _shadowVisual);
        }

        /// <summary>
        /// Handles the Unloaded event of the CompositionShadow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CompositionShadow_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= CompositionShadow_Loaded;
            SizeChanged -= CompositionShadow_SizeChanged;
        }

        /// <summary>
        /// Handles the Loaded event of the CompositionShadow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CompositionShadow_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigureShadowVisualForCastingElement();
        }

        /// <summary>
        /// The blur radius of the drop shadow.
        /// </summary>
        /// <value>The blur radius.</value>
        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        /// <summary>
        /// The FrameworkElement that this <see cref="CompositionShadow" /> uses to create the mask for the
        /// underlying <see cref="Windows.UI.Composition.DropShadow" />.
        /// </summary>
        /// <value>The casting element.</value>
        public FrameworkElement CastingElement
        {
            get => _castingElement;

            set
            {
                if (_castingElement != null)
                {
                    _castingElement.SizeChanged -= CompositionShadow_SizeChanged;
                }

                _castingElement = value;
                _castingElement.SizeChanged += CompositionShadow_SizeChanged;

                ConfigureShadowVisualForCastingElement();
            }
        }

        /// <summary>
        /// The color of the drop shadow.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Exposes the underlying composition object to allow custom Windows.UI.Composition animations.
        /// </summary>
        /// <value>The drop shadow.</value>
        public DropShadow DropShadow => _dropShadow;

        /// <summary>
        /// Exposes the underlying SpriteVisual to allow custom animations and transforms.
        /// </summary>
        /// <value>The visual.</value>
        public SpriteVisual Visual => _shadowVisual;

        /// <summary>
        /// The mask of the underlying <see cref="Windows.UI.Composition.DropShadow" />.
        /// Allows for a custom <see cref="Windows.UI.Composition.CompositionBrush" /> to be set.
        /// </summary>
        /// <value>The mask.</value>
        public CompositionBrush Mask
        {
            get => _dropShadow.Mask;
            set => _dropShadow.Mask = value;
        }

        /// <summary>
        /// The x offset of the drop shadow.
        /// </summary>
        /// <value>The offset x.</value>
        public double OffsetX
        {
            get => (double)GetValue(OffsetXProperty);
            set => SetValue(OffsetXProperty, value);
        }

        /// <summary>
        /// The y offset of the drop shadow.
        /// </summary>
        /// <value>The offset y.</value>
        public double OffsetY
        {
            get => (double)GetValue(OffsetYProperty);
            set => SetValue(OffsetYProperty, value);
        }

        /// <summary>
        /// The z offset of the drop shadow.
        /// </summary>
        /// <value>The offset z.</value>
        public double OffsetZ
        {
            get => (double)GetValue(OffsetZProperty);
            set => SetValue(OffsetZProperty, value);
        }

        /// <summary>
        /// The opacity of the drop shadow.
        /// </summary>
        /// <value>The shadow opacity.</value>
        public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, value);
        }

        /// <summary>
        /// Handles the <see cref="E:BlurRadiusChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CompositionShadow)d).OnBlurRadiusChanged((double)e.NewValue);
        }

        /// <summary>
        /// Handles the <see cref="E:ColorChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CompositionShadow)d).OnColorChanged((Color)e.NewValue);
        }

        /// <summary>
        /// Handles the <see cref="E:OffsetXChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CompositionShadow)d).OnOffsetXChanged((double)e.NewValue);
        }

        /// <summary>
        /// Handles the <see cref="E:OffsetYChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CompositionShadow)d).OnOffsetYChanged((double)e.NewValue);
        }

        /// <summary>
        /// Handles the <see cref="E:OffsetZChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOffsetZChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CompositionShadow)d).OnOffsetZChanged((double)e.NewValue);
        }

        /// <summary>
        /// Handles the <see cref="E:ShadowOpacityChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CompositionShadow)d).OnShadowOpacityChanged((double)e.NewValue);
        }

        /// <summary>
        /// Handles the SizeChanged event of the CompositionShadow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void CompositionShadow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateShadowSize();
        }

        /// <summary>
        /// Configures the shadow visual for casting element.
        /// </summary>
        private void ConfigureShadowVisualForCastingElement()
        {
            UpdateShadowMask();

            UpdateShadowSize();
        }

        /// <summary>
        /// Called when [blur radius changed].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnBlurRadiusChanged(double newValue)
        {
            _dropShadow.BlurRadius = (float)newValue;
        }

        /// <summary>
        /// Called when [color changed].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnColorChanged(Color newValue)
        {
            _dropShadow.Color = newValue;
        }

        /// <summary>
        /// Called when [offset x changed].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnOffsetXChanged(double newValue)
        {
            UpdateShadowOffset((float)newValue, _dropShadow.Offset.Y, _dropShadow.Offset.Z);
        }

        /// <summary>
        /// Called when [offset y changed].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnOffsetYChanged(double newValue)
        {
            UpdateShadowOffset(_dropShadow.Offset.X, (float)newValue, _dropShadow.Offset.Z);
        }

        /// <summary>
        /// Called when [offset z changed].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnOffsetZChanged(double newValue)
        {
            UpdateShadowOffset(_dropShadow.Offset.X, _dropShadow.Offset.Y, (float)newValue);
        }

        /// <summary>
        /// Called when [shadow opacity changed].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnShadowOpacityChanged(double newValue)
        {
            _dropShadow.Opacity = (float)newValue;
        }

        /// <summary>
        /// Updates the shadow mask.
        /// </summary>
        private void UpdateShadowMask()
        {
            if (_castingElement != null)
            {
                CompositionBrush mask = null;
                if (_castingElement is Image)
                {
                    mask = ((Image)_castingElement).GetAlphaMask();
                }
                else if (_castingElement is Shape)
                {
                    mask = ((Shape)_castingElement).GetAlphaMask();
                }
                else if (_castingElement is TextBlock)
                {
                    mask = ((TextBlock)_castingElement).GetAlphaMask();
                }

                _dropShadow.Mask = mask;
            }
            else
            {
                _dropShadow.Mask = null;
            }
        }

        /// <summary>
        /// Updates the shadow offset.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        private void UpdateShadowOffset(float x, float y, float z)
        {
            _dropShadow.Offset = new Vector3(x, y, z);
        }

        /// <summary>
        /// Updates the size of the shadow.
        /// </summary>
        private void UpdateShadowSize()
        {
            var newSize = new Vector2((float)ActualWidth, (float)ActualHeight);
            if (_castingElement != null)
            {
                newSize = new Vector2((float)_castingElement.ActualWidth, (float)_castingElement.ActualHeight);
            }

            _shadowVisual.Size = newSize;
        }

        /// <summary>
        /// The casting element
        /// </summary>
        private FrameworkElement _castingElement;
        /// <summary>
        /// The drop shadow
        /// </summary>
        private readonly DropShadow _dropShadow;
        /// <summary>
        /// The shadow visual
        /// </summary>
        private readonly SpriteVisual _shadowVisual;
    }
}
