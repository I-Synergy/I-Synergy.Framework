using Microsoft.UI.Xaml;
using RadioButton = Microsoft.UI.Xaml.Controls.RadioButton;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Class IconRadioButton.
/// Implements the <see cref="RadioButton" />
/// </summary>
/// <seealso cref="RadioButton" />
public partial class IconRadioButton : RadioButton
{
    /// <summary>
    /// The path icon property
    /// </summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(IconRadioButton), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the path icon.
    /// </summary>
    /// <value>The path icon.</value>
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
