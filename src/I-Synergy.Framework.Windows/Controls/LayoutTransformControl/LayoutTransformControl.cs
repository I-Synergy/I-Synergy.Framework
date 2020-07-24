using ISynergy.Framework.Windows.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// Control that implements support for transformations as if applied by LayoutTransform.
    /// </summary>
    [ContentProperty(Name = "Child")]
    public class LayoutTransformControl : Control
    {
        #region Properties
        /// <summary>
        /// Gets or sets the single child of the LayoutTransformControl.
        /// </summary>
        /// <value>The child.</value>
        /// <remarks>Corresponds to WPF's Decorator.Child
        /// property.</remarks>
        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        /// <summary>
        /// Identifies the ChildProperty.
        /// </summary>
        public static readonly DependencyProperty ChildProperty = DependencyProperty.
            Register(
                "Child",
                typeof(FrameworkElement),
                typeof(LayoutTransformControl),
                new PropertyMetadata(null, ChildChanged));

        /// <summary>
        /// Handle changes to the child dependency property.
        /// </summary>
        /// <param name="o">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private static void ChildChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutTransformControl)o).OnChildChanged((FrameworkElement)e.NewValue);
        }

        /// <summary>
        /// Updates content when the child property is changed.
        /// </summary>
        /// <param name="newContent">The new child.</param>
        private void OnChildChanged(FrameworkElement newContent)
        {
            if (_layoutRoot != null)
            {
                // Clear current child
                _layoutRoot.Children.Clear();
                if (newContent != null)
                {
                    // Add the new child to the tree
                    _layoutRoot.Children.Add(newContent);
                }

                // New child means re-layout is necessary
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the Transform of the LayoutTransformControl.
        /// </summary>
        /// <value>The transform.</value>
        /// <remarks>Corresponds to UIElement.RenderTransform.</remarks>
        public Transform Transform
        {
            get { return (Transform)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        /// <summary>
        /// Identifies the TransformProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty TransformProperty = DependencyProperty
            .Register(
                "Transform",
                typeof(Transform),
                typeof(LayoutTransformControl),
                new PropertyMetadata(null, TransformChanged));

        /// <summary>
        /// Handles changes to the Transform DependencyProperty.
        /// </summary>
        /// <param name="o">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private static void TransformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutTransformControl)o).OnTransformChanged(
                e.OldValue as Transform,
                (Transform)e.NewValue);
        }

        /// <summary>
        /// Processes the transform when the transform is changed.
        /// </summary>
        /// <param name="oldValue">The old transform</param>
        /// <param name="newValue">The transform to process.</param>
        private void OnTransformChanged(Transform oldValue, Transform newValue)
        {
            if (oldValue != null)
            {
                UnsubscribeFromTransformPropertyChanges(oldValue);
            }

            if (newValue != null)
            {
                SubscribeToTransformPropertyChanges(newValue);
            }

            ProcessTransform();
        }

        /// <summary>
        /// Unsubscribes from transform property changes.
        /// </summary>
        /// <param name="transform">The transform.</param>
        private void UnsubscribeFromTransformPropertyChanges(Transform transform)
        {
            var propertyChangeEventSources =
                _transformPropertyChangeEventSources[transform];

            foreach (var propertyChangeEventSource in propertyChangeEventSources)
            {
                propertyChangeEventSource.ValueChanged -= OnTransformPropertyChanged;
            }

            _transformPropertyChangeEventSources.Remove(transform);
        }

        /// <summary>
        /// Subscribes to transform property changes.
        /// </summary>
        /// <param name="transform">The transform.</param>
        private void SubscribeToTransformPropertyChanges(Transform transform)
        {
            if (transform is TransformGroup transformGroup)
            {
                foreach (var childTransform in transformGroup.Children)
                {
                    SubscribeToTransformPropertyChanges(childTransform);
                }

                return;
            }

            var propertyChangeEventSources = new List<PropertyChangeEventSource<double>>();

            _transformPropertyChangeEventSources.Add(transform, propertyChangeEventSources);

            if (transform is RotateTransform rotateTransform)
            {
                var anglePropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        rotateTransform,
                        "Angle");
                anglePropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(anglePropertyChangeEventSource);
                return;
            }

            if (transform is ScaleTransform scaleTransform)
            {
                var scaleXPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        scaleTransform,
                        "ScaleX");
                scaleXPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(scaleXPropertyChangeEventSource);
                var scaleYPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        scaleTransform,
                        "ScaleY");
                scaleYPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(scaleYPropertyChangeEventSource);
                return;
            }

            if (transform is SkewTransform skewTransform)
            {
                var angleXPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        skewTransform,
                        "AngleX");
                angleXPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(angleXPropertyChangeEventSource);
                var angleYPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        skewTransform,
                        "AngleY");
                angleYPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(angleYPropertyChangeEventSource);
                return;
            }

            if (transform is MatrixTransform matrixTransform)
            {
                var matrixPropertyChangeEventSource =
                    new PropertyChangeEventSource<double>(
                        matrixTransform,
                        "Matrix");
                matrixPropertyChangeEventSource.ValueChanged +=
                    OnTransformPropertyChanged;
                propertyChangeEventSources.Add(matrixPropertyChangeEventSource);
            }
        }

        /// <summary>
        /// Called when a property of a Transform changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnTransformPropertyChanged(object sender, double e)
        {
            TransformUpdated();
        }
        #endregion

        /// <summary>
        /// Value used to work around double arithmetic rounding issues.
        /// </summary>
        private const double AcceptableDelta = 0.0001;

        /// <summary>
        /// Value used to work around double arithmetic rounding issues.
        /// </summary>
        private const int DecimalsAfterRound = 4;

        /// <summary>
        /// List of property change event sources for events when properties of the Transform tree change
        /// </summary>
        private readonly Dictionary<Transform, List<PropertyChangeEventSource<double>>>
            _transformPropertyChangeEventSources = new Dictionary
                <Transform, List<PropertyChangeEventSource<double>>>();

        /// <summary>
        /// Host panel for Child element.
        /// </summary>
        private Panel _layoutRoot;

        /// <summary>
        /// RenderTransform/MatrixTransform applied to layout root.
        /// </summary>
        private MatrixTransform _matrixTransform;

        /// <summary>
        /// Transformation matrix corresponding to matrix transform.
        /// </summary>
        private Matrix _transformation;

        /// <summary>
        /// Actual DesiredSize of Child element.
        /// </summary>
        private Size _childActualSize = Size.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutTransformControl" /> class.
        /// </summary>
        public LayoutTransformControl()
        {
            DefaultStyleKey = typeof(LayoutTransformControl);

            // Can't tab to LayoutTransformControl
            IsTabStop = false;

            // Disable layout rounding because its rounding of values confuses things.
            UseLayoutRounding = false;
        }

        /// <summary>
        /// Called whenever the control's template changes.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // Save existing content and remove it from the visual tree
            var savedContent = Child;
            Child = null;

            // Apply new template
            base.OnApplyTemplate();

            // Find template parts
            _layoutRoot = GetTemplateChild("LayoutRoot") as Panel;
            _matrixTransform = GetTemplateChild("MatrixTransform") as MatrixTransform;

            // RestoreAsync saved content
            Child = savedContent;

            // Apply the current transform
            TransformUpdated();
        }

        /// <summary>
        /// Notifies the LayoutTransformControl that some aspect of its Transform property has changed.
        /// </summary>
        /// <remarks>Call this to update the LayoutTransform in cases where
        /// LayoutTransformControl wouldn't otherwise know to do so.</remarks>
        public void TransformUpdated()
        {
            ProcessTransform();
        }

        /// <summary>
        /// Return true if Size a is smaller than Size b in either dimension.
        /// </summary>
        /// <param name="a">The left size.</param>
        /// <param name="b">The right size.</param>
        /// <returns>A value indicating whether the left size is smaller than
        /// the right.</returns>
        private static bool IsSizeSmaller(Size a, Size b)
        {
            // WPF equivalent of following code:
            // return ((a.Width < b.Width) || (a.Height < b.Height));
            return (a.Width + AcceptableDelta < b.Width) || (a.Height + AcceptableDelta < b.Height);
        }

        /// <summary>
        /// Processes the current transform to determine the corresponding
        /// matrix.
        /// </summary>
        private void ProcessTransform()
        {
            // Get the transform matrix and apply it
            _transformation = GetTransformMatrix(Transform).Round(DecimalsAfterRound);

            if (_matrixTransform != null)
            {
                _matrixTransform.Matrix = _transformation;
            }

            // New transform means re-layout is necessary
            InvalidateMeasure();
        }

        /// <summary>
        /// Walks the Transform and returns the corresponding matrix.
        /// </summary>
        /// <param name="transform">The transform to create a matrix for.</param>
        /// <returns>The matrix calculated from the transform.</returns>
        /// <exception cref="NotSupportedException">CompositeTransforms are not supported (yet) by the LayoutTransformControl.</exception>
        private Matrix GetTransformMatrix(Transform transform)
        {
            if (transform != null)
            {
                // WPF equivalent of this entire method (why oh why only WPF...):
                // return transform.Value;

                // Process the TransformGroup

                if (transform is TransformGroup transformGroup)
                {
                    var groupMatrix = Matrix.Identity;

                    foreach (var child in transformGroup.Children)
                    {
                        groupMatrix = groupMatrix.Multiply(GetTransformMatrix(child));
                    }

                    return groupMatrix;
                }

                // Process the RotateTransform

                if (transform is RotateTransform rotateTransform)
                {
                    return rotateTransform.GetMatrix();
                }

                // Process the ScaleTransform

                if (transform is ScaleTransform scaleTransform)
                {
                    return scaleTransform.GetMatrix();
                }

                // Process the SkewTransform

                if (transform is SkewTransform skewTransform)
                {
                    return skewTransform.GetMatrix();
                }

                // Process the MatrixTransform
                if (transform is MatrixTransform matrixTransform)
                {
                    return matrixTransform.Matrix;
                }

                if (transform is CompositeTransform)
                {
                    throw new NotSupportedException("CompositeTransforms are not supported (yet) by the LayoutTransformControl.");
                }

                // TranslateTransform has no effect in LayoutTransform
            }

            // Fall back to no-op transformation
            return Matrix.Identity;
        }

        /// <summary>
        /// Provides the behavior for the "Measure" pass of layout.
        /// </summary>
        /// <param name="availableSize">The available size that this element can
        /// give to child elements. Infinity can be specified as a value to
        /// indicate that the element will size to whatever content is available.</param>
        /// <returns>The size that this element determines it needs during
        /// layout, based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var child = Child;
            if (_layoutRoot is null || child is null)
            {
                // No content, no size
                return Size.Empty;
            }

            Size measureSize;
            if (_childActualSize == Size.Empty)
            {
                // Determine the largest size after the transformation
                measureSize = ComputeLargestTransformedSize(availableSize);
            }
            else
            {
                // Previous measure/arrange pass determined that Child.DesiredSize was larger than believed.
                measureSize = _childActualSize;
            }

            // Perform a mesaure on the _layoutRoot (containing Child)
            _layoutRoot.Measure(measureSize);

            // Transform DesiredSize to find its width/height
            var transformedDesiredRect = _transformation.RectTransform(new Rect(0, 0, _layoutRoot.DesiredSize.Width, _layoutRoot.DesiredSize.Height));
            var transformedDesiredSize = new Size(transformedDesiredRect.Width, transformedDesiredRect.Height);

            // Return result to allocate enough space for the transformation
            return transformedDesiredSize;
        }

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of layout.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this
        /// element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var child = Child;
            if (_layoutRoot is null || child is null)
            {
                // No child, use whatever was given
                return finalSize;
            }

            // Determine the largest available size after the transformation
            var finalSizeTransformed = ComputeLargestTransformedSize(finalSize);
            if (IsSizeSmaller(finalSizeTransformed, _layoutRoot.DesiredSize))
            {
                // Some elements do not like being given less space than they asked for (ex: TextBlock)
                // Bump the working size up to do the right thing by them
                finalSizeTransformed = _layoutRoot.DesiredSize;
            }

            // Transform the working size to find its width/height
            var transformedRect = _transformation.RectTransform(new Rect(0, 0, finalSizeTransformed.Width, finalSizeTransformed.Height));

            // Create the Arrange rect to center the transformed content
            var finalRect = new Rect(
                -transformedRect.Left + ((finalSize.Width - transformedRect.Width) / 2),
                -transformedRect.Top + ((finalSize.Height - transformedRect.Height) / 2),
                finalSizeTransformed.Width,
                finalSizeTransformed.Height);

            // Perform an Arrange on _layoutRoot (containing Child)
            _layoutRoot.Arrange(finalRect);

            // This is the first opportunity to find out the Child's true DesiredSize
            if (IsSizeSmaller(finalSizeTransformed, child.RenderSize) && (Size.Empty == _childActualSize))
            {
                // Unfortunately, all the work so far is invalid because the wrong DesiredSize was used
                // Make a note of the actual DesiredSize
                _childActualSize = new Size(child.ActualWidth, child.ActualHeight);

                // Force a new measure/arrange pass
                InvalidateMeasure();
            }
            else
            {
                // Clear the "need to measure/arrange again" flag
                _childActualSize = Size.Empty;
            }

            // Return result to perform the transformation
            return finalSize;
        }

        /// <summary>
        /// Computes the largest usable size after applying the transformation to the specified bounds.
        /// </summary>
        /// <param name="arrangeBounds">The size to arrange within.</param>
        /// <returns>The size required.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Closely corresponds to WPF's FrameworkElement.FindMaximalAreaLocalSpaceRect.")]
        private Size ComputeLargestTransformedSize(Size arrangeBounds)
        {
            // Computed largest transformed size
            var computedSize = Size.Empty;

            // Detect infinite bounds and constrain the scenario
            var infiniteWidth = double.IsInfinity(arrangeBounds.Width);

            if (infiniteWidth)
            {
                arrangeBounds.Width = arrangeBounds.Height;
            }

            var infiniteHeight = double.IsInfinity(arrangeBounds.Height);

            if (infiniteHeight)
            {
                arrangeBounds.Height = arrangeBounds.Width;
            }

            // Capture the matrix parameters
            var a = _transformation.M11;
            var b = _transformation.M12;
            var c = _transformation.M21;
            var d = _transformation.M22;

            // Compute maximum possible transformed width/height based on starting width/height
            // These constraints define two lines in the positive x/y quadrant
            var maxWidthFromWidth = Math.Abs(arrangeBounds.Width / a);
            var maxHeightFromWidth = Math.Abs(arrangeBounds.Width / c);
            var maxWidthFromHeight = Math.Abs(arrangeBounds.Height / b);
            var maxHeightFromHeight = Math.Abs(arrangeBounds.Height / d);

            // The transformed width/height that maximize the area under each segment is its midpoint
            // At most one of the two midpoints will satisfy both constraints
            var idealWidthFromWidth = maxWidthFromWidth / 2;
            var idealHeightFromWidth = maxHeightFromWidth / 2;
            var idealWidthFromHeight = maxWidthFromHeight / 2;
            var idealHeightFromHeight = maxHeightFromHeight / 2;

            // Compute slope of both constraint lines
            var slopeFromWidth = -(maxHeightFromWidth / maxWidthFromWidth);
            var slopeFromHeight = -(maxHeightFromHeight / maxWidthFromHeight);

            if (arrangeBounds.Width == 0 || arrangeBounds.Height == 0)
            {
                // Check for empty bounds
                computedSize = new Size(0, 0);
            }
            else if (infiniteWidth && infiniteHeight)
            {
                // Check for completely unbound scenario
                computedSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            }
            else if (!_transformation.HasInverse())
            {
                // Check for singular matrix
                computedSize = new Size(0, 0);
            }
            else if (b == 0 || c == 0)
            {
                // Check for 0/180 degree special cases
                var maxHeight = infiniteHeight ? double.PositiveInfinity : maxHeightFromHeight;
                var maxWidth = infiniteWidth ? double.PositiveInfinity : maxWidthFromWidth;

                if (b == 0 && c == 0)
                {
                    // No constraints
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (b == 0)
                {
                    // Constrained by width
                    var computedHeight = Math.Min(idealHeightFromWidth, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs(c * computedHeight / a),
                        computedHeight);
                }
                else if (c == 0)
                {
                    // Constrained by height
                    var computedWidth = Math.Min(idealWidthFromHeight, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs(b * computedWidth / d));
                }
            }
            else if (a == 0 || d == 0)
            {
                // Check for 90/270 degree special cases
                var maxWidth = infiniteHeight ? double.PositiveInfinity : maxWidthFromHeight;
                var maxHeight = infiniteWidth ? double.PositiveInfinity : maxHeightFromWidth;

                if (a == 0 && d == 0)
                {
                    // No constraints
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (a == 0)
                {
                    // Constrained by width
                    var computedHeight = Math.Min(idealHeightFromHeight, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs(d * computedHeight / b),
                        computedHeight);
                }
                else if (d == 0)
                {
                    // Constrained by height.
                    var computedWidth = Math.Min(idealWidthFromWidth, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs(a * computedWidth / c));
                }
            }
            else if (idealHeightFromWidth <= ((slopeFromHeight * idealWidthFromWidth) + maxHeightFromHeight))
            {
                // Check the width midpoint for viability (by being below the height constraint line).
                computedSize = new Size(idealWidthFromWidth, idealHeightFromWidth);
            }
            else if (idealHeightFromHeight <= ((slopeFromWidth * idealWidthFromHeight) + maxHeightFromWidth))
            {
                // Check the height midpoint for viability (by being below the width constraint line).
                computedSize = new Size(idealWidthFromHeight, idealHeightFromHeight);
            }
            else
            {
                // Neither midpoint is viable; use the intersection of the two constraint lines instead.

                // Compute width by setting heights equal (m1*x+c1=m2*x+c2).
                var computedWidth = (maxHeightFromHeight - maxHeightFromWidth) / (slopeFromWidth - slopeFromHeight);

                // Compute height from width constraint line (y=m*x+c; using height would give same result).
                computedSize = new Size(
                    computedWidth,
                    (slopeFromWidth * computedWidth) + maxHeightFromWidth);
            }

            return computedSize;
        }
    }
}
