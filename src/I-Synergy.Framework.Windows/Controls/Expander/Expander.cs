using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Controls
{
    /// <summary>
    /// Expand direction enumeration
    /// </summary>
    public enum ExpandDirection
    {
        /// <summary>
        /// Down
        /// </summary>
        Down,

        /// <summary>
        /// Up
        /// </summary>
        Up,

        /// <summary>
        /// Left
        /// </summary>
        Left,

        /// <summary>
        /// Right
        /// </summary>
        Right
    }

    /// <summary>
    /// The <see cref="Expander"/> control allows user to show/hide content based on a boolean state
    /// </summary>
    [TemplateVisualState(Name = StateContentLeftDirection, GroupName = ExpandDirectionGroupStateContent)]
    [TemplateVisualState(Name = StateContentDownDirection, GroupName = ExpandDirectionGroupStateContent)]
    [TemplateVisualState(Name = StateContentRightDirection, GroupName = ExpandDirectionGroupStateContent)]
    [TemplateVisualState(Name = StateContentUpDirection, GroupName = ExpandDirectionGroupStateContent)]
    [TemplateVisualState(Name = StateContentVisibleLeft, GroupName = DisplayModeAndDirectionStatesGroupStateContent)]
    [TemplateVisualState(Name = StateContentVisibleDown, GroupName = DisplayModeAndDirectionStatesGroupStateContent)]
    [TemplateVisualState(Name = StateContentVisibleRight, GroupName = DisplayModeAndDirectionStatesGroupStateContent)]
    [TemplateVisualState(Name = StateContentVisibleUp, GroupName = DisplayModeAndDirectionStatesGroupStateContent)]
    [TemplateVisualState(Name = StateContentCollapsedLeft, GroupName = DisplayModeAndDirectionStatesGroupStateContent)]
    [TemplateVisualState(Name = StateContentCollapsedDown, GroupName = DisplayModeAndDirectionStatesGroupStateContent)]
    [TemplateVisualState(Name = StateContentCollapsedRight, GroupName = DisplayModeAndDirectionStatesGroupStateContent)]
    [TemplateVisualState(Name = StateContentCollapsedUp, GroupName = DisplayModeAndDirectionStatesGroupStateContent)]
    [TemplatePart(Name = RootGridPart, Type = typeof(Grid))]
    [TemplatePart(Name = ExpanderToggleButtonPart, Type = typeof(ToggleButton))]
    [TemplatePart(Name = LayoutTransformerPart, Type = typeof(LayoutTransformControl))]
    [ContentProperty(Name = "Content")]
    public class Expander : HeaderedContentControl
    {
        #region Constants
        /// <summary>
        /// Key of the VisualStateGroup that trigger display mode (visible/collapsed) and direction content
        /// </summary>
        private const string DisplayModeAndDirectionStatesGroupStateContent = "DisplayModeAndDirectionStates";

        /// <summary>
        /// Key of the VisualState when expander is visible and expander direction is set to Left
        /// </summary>
        private const string StateContentVisibleLeft = "VisibleLeft";

        /// <summary>
        /// Key of the VisualState when expander is visible and expander direction is set to Down
        /// </summary>
        private const string StateContentVisibleDown = "VisibleDown";

        /// <summary>
        /// Key of the VisualState when expander is visible and expander direction is set to Right
        /// </summary>
        private const string StateContentVisibleRight = "VisibleRight";

        /// <summary>
        /// Key of the VisualState when expander is visible and expander direction is set to Up
        /// </summary>
        private const string StateContentVisibleUp = "VisibleUp";

        /// <summary>
        /// Key of the VisualState when expander is collapsed and expander direction is set to Left
        /// </summary>
        private const string StateContentCollapsedLeft = "CollapsedLeft";

        /// <summary>
        /// Key of the VisualState when expander is collapsed and expander direction is set to Down
        /// </summary>
        private const string StateContentCollapsedDown = "CollapsedDown";

        /// <summary>
        /// Key of the VisualState when expander is collapsed and expander direction is set to Right
        /// </summary>
        private const string StateContentCollapsedRight = "CollapsedRight";

        /// <summary>
        /// Key of the VisualState when expander is collapsed and expander direction is set to Up
        /// </summary>
        private const string StateContentCollapsedUp = "CollapsedUp";

        /// <summary>
        /// Key of the UI Element that toggle IsExpanded property
        /// </summary>
        private const string ExpanderToggleButtonPart = "PART_ExpanderToggleButton";

        /// <summary>
        /// Key of the UI Element that contains the content of the control that is expanded/collapsed
        /// </summary>
        private const string MainContentPart = "PART_MainContent";

        /// <summary>
        /// Key of the VisualStateGroup that set expander direction of the control
        /// </summary>
        private const string ExpandDirectionGroupStateContent = "ExpandDirectionStates";

        /// <summary>
        /// Key of the VisualState when expander direction is set to Left
        /// </summary>
        private const string StateContentLeftDirection = "LeftDirection";

        /// <summary>
        /// Key of the VisualState when expander direction is set to Down
        /// </summary>
        private const string StateContentDownDirection = "DownDirection";

        /// <summary>
        /// Key of the VisualState when expander direction is set to Right
        /// </summary>
        private const string StateContentRightDirection = "RightDirection";

        /// <summary>
        /// Key of the VisualState when expander direction is set to Up
        /// </summary>
        private const string StateContentUpDirection = "UpDirection";

        /// <summary>
        /// Key of the UI Element that contains the content of the entire control
        /// </summary>
        private const string RootGridPart = "PART_RootGrid";

        /// <summary>
        /// Key of the UI Element that contains the content of the LayoutTransformer (of the expander button)
        /// </summary>
        private const string LayoutTransformerPart = "PART_LayoutTransformer";
        #endregion

        #region Events
        /// <summary>
        /// Fires when the expander is opened
        /// </summary>
        public event EventHandler Expanded;

        /// <summary>
        /// Fires when the expander is closed
        /// </summary>
        public event EventHandler Collapsed;
        #endregion

        #region Properties
        /// <summary>
        /// Identifies the <see cref="IsExpanded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(Expander), new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ExpandDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(nameof(ExpandDirection), typeof(ExpandDirection), typeof(Expander), new PropertyMetadata(ExpandDirection.Down, OnExpandDirectionChanged));

        /// <summary>
        /// Identifies the <see cref="ContentOverlay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentOverlayProperty =
            DependencyProperty.Register(nameof(ContentOverlay), typeof(UIElement), typeof(Expander), new PropertyMetadata(default(UIElement)));

        /// <summary>
        /// Identifies the <see cref="HeaderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(nameof(HeaderStyle), typeof(Style), typeof(Expander), new PropertyMetadata(default(Style)));

        /// <summary>
        /// Gets or sets a value indicating whether the content of the control is opened/visible or closed/hidden.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Expand Direction of the control.
        /// </summary>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ContentOverlay of the control.
        /// </summary>
        public UIElement ContentOverlay
        {
            get { return (UIElement)GetValue(ContentOverlayProperty); }
            set { SetValue(ContentOverlayProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value for the style to use for the Header of the Expander.
        /// </summary>
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expander = d as Expander;

            bool isExpanded = (bool)e.NewValue;
            if (isExpanded)
            {
                expander.ExpandControl();
            }
            else
            {
                expander.CollapseControl();
            }
        }

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
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Expander"/> class.
        /// </summary>
        public Expander()
        {
            DefaultStyleKey = typeof(Expander);
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var button = (ToggleButton)GetTemplateChild(ExpanderToggleButtonPart);

            if (button != null)
            {
                button.KeyDown -= ExpanderToggleButtonPart_KeyDown;
                button.KeyDown += ExpanderToggleButtonPart_KeyDown;
            }

            OnExpandDirectionChanged(false);
            OnDisplayModeOrIsExpandedChanged(false);
        }

        /// <summary>
        /// Called when control is expanded
        /// </summary>
        /// <param name="args">EventArgs</param>
        protected virtual void OnExpanded(EventArgs args)
        {
            Expanded?.Invoke(this, args);
        }

        /// <summary>
        /// Called when control is collapsed
        /// </summary>
        /// <param name="args">EventArgs</param>
        protected virtual void OnCollapsed(EventArgs args)
        {
            Collapsed?.Invoke(this, args);
        }

        private void ExpanderToggleButtonPart_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
            {
                return;
            }


            if (!(sender is ToggleButton button))
            {
                return;
            }

            IsExpanded = !IsExpanded;

            e.Handled = true;
        }

        private void ExpandControl()
        {
            OnDisplayModeOrIsExpandedChanged();
            OnExpanded(EventArgs.Empty);
        }

        private void CollapseControl()
        {
            OnDisplayModeOrIsExpandedChanged();
            OnCollapsed(EventArgs.Empty);
        }

        /// <summary>
        /// Called when the ExpandDirection on Expander changes
        /// </summary>
        private void OnExpandDirectionChanged(bool useTransitions = true)
        {
            var button = (ToggleButton)GetTemplateChild(ExpanderToggleButtonPart);

            if (button == null)
            {
                return;
            }

            UpdateDisplayModeOrExpanderDirection(useTransitions);

            switch (ExpandDirection)
            {
                case ExpandDirection.Left:
                    VisualStateManager.GoToState(button, StateContentLeftDirection, useTransitions);
                    break;
                case ExpandDirection.Down:
                    VisualStateManager.GoToState(button, StateContentDownDirection, useTransitions);
                    break;
                case ExpandDirection.Right:
                    VisualStateManager.GoToState(button, StateContentRightDirection, useTransitions);
                    break;
                case ExpandDirection.Up:
                    VisualStateManager.GoToState(button, StateContentUpDirection, useTransitions);
                    break;
            }

            // Re-execute animation on expander toggle button (to set correct arrow rotation)
            VisualStateManager.GoToState(button, "Normal", true);
            if (button.IsChecked.HasValue && button.IsChecked.Value)
            {
                VisualStateManager.GoToState(button, "Checked", true);
            }
        }

        private void OnDisplayModeOrIsExpandedChanged(bool useTransitions = true)
        {
            UpdateDisplayModeOrExpanderDirection(useTransitions);
        }

        private void UpdateDisplayModeOrExpanderDirection(bool useTransitions = true)
        {
            string visualState = null;

            switch (ExpandDirection)
            {
                case ExpandDirection.Left:
                    visualState = GetDisplayModeVisualState(StateContentCollapsedLeft, StateContentVisibleLeft);
                    break;
                case ExpandDirection.Down:
                    visualState = GetDisplayModeVisualState(StateContentCollapsedDown, StateContentVisibleDown);
                    break;
                case ExpandDirection.Right:
                    visualState = GetDisplayModeVisualState(StateContentCollapsedRight, StateContentVisibleRight);
                    break;
                case ExpandDirection.Up:
                    visualState = GetDisplayModeVisualState(StateContentCollapsedUp, StateContentVisibleUp);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(visualState))
            {
                VisualStateManager.GoToState(this, visualState, useTransitions);
            }
        }

        private string GetDisplayModeVisualState(string collapsedState, string visibleState)
        {
            return IsExpanded ? visibleState : collapsedState;
        }
    }
}