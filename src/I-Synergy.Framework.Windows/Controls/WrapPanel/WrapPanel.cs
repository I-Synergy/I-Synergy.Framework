using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// WrapPanel is a panel that position child control vertically or horizontally based on the orientation and when max width / max height is reached a new row (in case of horizontal) or column (in case of vertical) is created to fit new controls.
    /// </summary>
    public class WrapPanel : Panel
    {
        /// <summary>
        /// Struct UvMeasure
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("U = {U} V = {V}")]
        private struct UvMeasure
        {
            /// <summary>
            /// The zero
            /// </summary>
            internal static readonly UvMeasure Zero = default;

            /// <summary>
            /// Gets or sets the u.
            /// </summary>
            /// <value>The u.</value>
            internal double U { get; set; }

            /// <summary>
            /// Gets or sets the v.
            /// </summary>
            /// <value>The v.</value>
            internal double V { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="UvMeasure"/> struct.
            /// </summary>
            /// <param name="orientation">The orientation.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            public UvMeasure(Orientation orientation, double width, double height)
            {
                if (orientation == Orientation.Horizontal)
                {
                    U = width;
                    V = height;
                }
                else
                {
                    U = height;
                    V = width;
                }
            }
        }

        /// <summary>
        /// Gets or sets a uniform Horizontal distance (in pixels) between items when <see cref="Orientation" /> is set to Horizontal,
        /// or between columns of items when <see cref="Orientation" /> is set to Vertical.
        /// </summary>
        /// <value>The horizontal spacing.</value>
        public double HorizontalSpacing
        {
            get { return (double)GetValue(HorizontalSpacingProperty); }
            set { SetValue(HorizontalSpacingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalSpacing" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalSpacingProperty =
            DependencyProperty.Register(
                nameof(HorizontalSpacing),
                typeof(double),
                typeof(WrapPanel),
                new PropertyMetadata(0d, LayoutPropertyChanged));

        /// <summary>
        /// Gets or sets a uniform Vertical distance (in pixels) between items when <see cref="Orientation" /> is set to Vertical,
        /// or between rows of items when <see cref="Orientation" /> is set to Horizontal.
        /// </summary>
        /// <value>The vertical spacing.</value>
        public double VerticalSpacing
        {
            get { return (double)GetValue(VerticalSpacingProperty); }
            set { SetValue(VerticalSpacingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VerticalSpacing" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalSpacingProperty =
            DependencyProperty.Register(
                nameof(VerticalSpacing),
                typeof(double),
                typeof(WrapPanel),
                new PropertyMetadata(0d, LayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the orientation of the WrapPanel.
        /// Horizontal means that child controls will be added horizontally until the width of the panel is reached, then a new row is added to add new child controls.
        /// Vertical means that children will be added vertically until the height of the panel is reached, then a new column is added.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Orientation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(WrapPanel),
                new PropertyMetadata(Orientation.Horizontal, LayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        /// <value>The padding.</value>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// Identifies the Padding dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Padding" /> dependency property.</returns>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(WrapPanel),
                new PropertyMetadata(default(Thickness), LayoutPropertyChanged));

        /// <summary>
        /// Layouts the property changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WrapPanel wp)
            {
                wp.InvalidateMeasure();
                wp.InvalidateArrange();
            }
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize.Width = availableSize.Width - Padding.Left - Padding.Right;
            availableSize.Height = availableSize.Height - Padding.Top - Padding.Bottom;
            var totalMeasure = UvMeasure.Zero;
            var parentMeasure = new UvMeasure(Orientation, availableSize.Width, availableSize.Height);
            var spacingMeasure = new UvMeasure(Orientation, HorizontalSpacing, VerticalSpacing);
            var lineMeasure = UvMeasure.Zero;

            foreach (var child in Children)
            {
                child.Measure(availableSize);

                var currentMeasure = new UvMeasure(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);

                if (parentMeasure.U >= currentMeasure.U + lineMeasure.U + spacingMeasure.U)
                {
                    lineMeasure.U += currentMeasure.U + spacingMeasure.U;
                    lineMeasure.V = Math.Max(lineMeasure.V, currentMeasure.V);
                }
                else
                {
                    // new line should be added
                    // to get the max U to provide it correctly to ui width ex: ---| or -----|
                    totalMeasure.U = Math.Max(lineMeasure.U, totalMeasure.U);
                    totalMeasure.V += lineMeasure.V + spacingMeasure.V;

                    // if the next new row still can handle more controls
                    if (parentMeasure.U > currentMeasure.U)
                    {
                        // set lineMeasure initial values to the currentMeasure to be calculated later on the new loop
                        lineMeasure = currentMeasure;
                    }

                    // the control will take one row alone
                    else
                    {
                        // validate the new control measures
                        totalMeasure.U = Math.Max(currentMeasure.U, totalMeasure.U);
                        totalMeasure.V += currentMeasure.V;

                        // add new empty line
                        lineMeasure = UvMeasure.Zero;
                    }
                }
            }

            // update value with the last line
            // if the the last loop is(parentMeasure.U > currentMeasure.U + lineMeasure.U) the total isn't calculated then calculate it
            // if the last loop is (parentMeasure.U > currentMeasure.U) the currentMeasure isn't added to the total so add it here
            // for the last condition it is zeros so adding it will make no difference
            // this way is faster than an if condition in every loop for checking the last item
            totalMeasure.U = Math.Max(lineMeasure.U, totalMeasure.U);
            totalMeasure.V += lineMeasure.V;

            totalMeasure.U = Math.Ceiling(totalMeasure.U);

            return Orientation == Orientation.Horizontal ? new Size(totalMeasure.U, totalMeasure.V) : new Size(totalMeasure.V, totalMeasure.U);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            var parentMeasure = new UvMeasure(Orientation, finalSize.Width, finalSize.Height);
            var spacingMeasure = new UvMeasure(Orientation, HorizontalSpacing, VerticalSpacing);
            var paddingStart = new UvMeasure(Orientation, Padding.Left, Padding.Top);
            var paddingEnd = new UvMeasure(Orientation, Padding.Right, Padding.Bottom);
            var position = new UvMeasure(Orientation, Padding.Left, Padding.Top);

            double currentV = 0;
            foreach (var child in Children)
            {
                var desiredMeasure = new UvMeasure(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                if ((desiredMeasure.U + position.U + paddingEnd.U) > parentMeasure.U)
                {
                    // next row!
                    position.U = paddingStart.U;
                    position.V += currentV + spacingMeasure.V;
                    currentV = 0;
                }

                // Place the item
                if (Orientation == Orientation.Horizontal)
                {
                    child.Arrange(new Rect(position.U, position.V, child.DesiredSize.Width, child.DesiredSize.Height));
                }
                else
                {
                    child.Arrange(new Rect(position.V, position.U, child.DesiredSize.Width, child.DesiredSize.Height));
                }

                // adjust the location for the next items
                position.U += desiredMeasure.U + spacingMeasure.U;
                currentV = Math.Max(desiredMeasure.V, currentV);
            }

            return finalSize;
        }
    }
}
