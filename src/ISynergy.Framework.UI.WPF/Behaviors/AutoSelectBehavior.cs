using ISynergy.Framework.UI.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Behaviors;

/// <summary>
/// Behavior for auto selection.
/// </summary>
public static class AutoSelectBehavior
{
    /// <summary>
    /// Gets the automatic selectable.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool GetAutoSelectable(DependencyObject obj)
    {
        return (bool)obj.GetValue(AutoSelectableProperty);
    }

    /// <summary>
    /// Sets the automatic selectable.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public static void SetAutoSelectable(DependencyObject obj, bool value)
    {
        obj.SetValue(AutoSelectableProperty, value);
    }

    /// <summary>
    /// The automatic selectable property
    /// </summary>
    public static readonly DependencyProperty AutoSelectableProperty =
        DependencyProperty.RegisterAttached(
            "AutoSelectable",
            typeof(bool),
            typeof(AutoSelectBehavior),
            new PropertyMetadata(false, AutoSelectableChangedHandler));

    /// <summary>
    /// Automatics the focusable changed handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void AutoSelectableChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextBox? textBox = null;

        if (d is TextBox control)
            textBox = control;
        else
            textBox = d.FindChild<TextBox>();

        if (e.NewValue != e.OldValue && textBox is not null)
        {
            if ((bool)e.NewValue)
            {
                textBox.GotFocus += OnGotFocusHandler;
            }
            else
            {
                textBox.GotFocus -= OnGotFocusHandler;
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="E:GotFocusHandler" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private static void OnGotFocusHandler(object sender, RoutedEventArgs e)
    {
        TextBox? textBox = null;

        if (sender is TextBox control)
            textBox = control;
        else
            textBox = (sender as DependencyObject)?.FindChild<TextBox>();

        textBox?.SelectAll();
    }
}
