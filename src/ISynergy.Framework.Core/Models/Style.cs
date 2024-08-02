using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Application style model.
/// </summary>
[Bindable(BindableSupport.Yes)]
public class Style : ObservableClass
{

    /// <summary>
    /// Gets or sets the Color property value.
    /// </summary>
    /// <value>The color.</value>
    public string Color
    {
        get => GetValue<string>();
        set => SetValue(value);
    }


    /// <summary>
    /// Gets or sets the Theme property value.
    /// </summary>
    /// <value>The theme.</value>
    public Themes Theme
    {
        get => GetValue<Themes>();
        set => SetValue(value);
    }

    /// <summary>
    /// Prevents a default instance of the <see cref="Style"/> class from being created.
    /// </summary>
    public Style()
    {
        Color = ThemeColors.Default;
        Theme = Themes.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Style"/> class.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="theme">The theme.</param>
    public Style(string color, Themes theme)
    {
        Color = color;
        Theme = theme;
    }
}
