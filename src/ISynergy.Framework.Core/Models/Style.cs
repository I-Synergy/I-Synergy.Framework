using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Application style model.
/// </summary>
public class Style : ObservableClass
{

    /// <summary>
    /// Gets or sets the Color property value.
    /// </summary>
    /// <value>The color.</value>
    public ThemeColors Color
    {
        get => GetValue<ThemeColors>();
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
    public Style(ThemeColors color, Themes theme)
    {
        Color = color;
        Theme = theme;
    }
}
