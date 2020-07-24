using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// The <see cref="Expander" /> control allows user to show/hide content based on a boolean state
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
    public partial class Expander : HeaderedContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Expander" /> class.
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

        /// <summary>
        /// Handles the KeyDown event of the ExpanderToggleButtonPart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Expands the control.
        /// </summary>
        private void ExpandControl()
        {
            OnDisplayModeOrIsExpandedChanged();
            OnExpanded(EventArgs.Empty);
        }

        /// <summary>
        /// Collapses the control.
        /// </summary>
        private void CollapseControl()
        {
            OnDisplayModeOrIsExpandedChanged();
            OnCollapsed(EventArgs.Empty);
        }

        /// <summary>
        /// Called when the ExpandDirection on Expander changes
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> [use transitions].</param>
        private void OnExpandDirectionChanged(bool useTransitions = true)
        {
            var button = (ToggleButton)GetTemplateChild(ExpanderToggleButtonPart);

            if (button is null)
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

        /// <summary>
        /// Called when [display mode or is expanded changed].
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> [use transitions].</param>
        private void OnDisplayModeOrIsExpandedChanged(bool useTransitions = true)
        {
            UpdateDisplayModeOrExpanderDirection(useTransitions);
        }

        /// <summary>
        /// Updates the display mode or expander direction.
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> [use transitions].</param>
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

        /// <summary>
        /// Gets the display state of the mode visual.
        /// </summary>
        /// <param name="collapsedState">State of the collapsed.</param>
        /// <param name="visibleState">State of the visible.</param>
        /// <returns>System.String.</returns>
        private string GetDisplayModeVisualState(string collapsedState, string visibleState)
        {
            return IsExpanded ? visibleState : collapsedState;
        }
    }
}
