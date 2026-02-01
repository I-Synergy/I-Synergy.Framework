using System.Windows;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Controls;

public class MaskedTextBox : TextBox
{
    /// <summary>
    /// Represents a mask/format for the textBox that the user must follow
    /// </summary>
    public static readonly DependencyProperty MaskProperty = DependencyProperty.RegisterAttached("Mask", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets mask value
    /// </summary>
    /// <param name="obj">control</param>
    /// <returns>mask value</returns>
    public static string GetMask(DependencyObject obj)
    {
        return (string)obj.GetValue(MaskProperty);
    }

    /// <summary>
    /// Sets textBox mask property which represents mask/format for the textBox that the user must follow
    /// </summary>
    /// <param name="obj">Control</param>
    /// <param name="value">Mask Value</param>
    public static void SetMask(DependencyObject obj, string value)
    {
        obj.SetValue(MaskProperty, value);
    }

    /// <summary>
    /// Determines if Masked input is enabled or disabled.
    /// </summary>
    public static readonly DependencyProperty IsMaskEnabledProperty = DependencyProperty.RegisterAttached("IsMaskEnabled", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(false));

    /// <summary>
    /// Gets the IsMaskEnabled property.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsMaskEnabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsMaskEnabledProperty);
    }

    /// <summary>
    /// Sets the IsMaskEnabled property.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetIsMaskEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(IsMaskEnabledProperty, value);
    }
}
