using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class ThemeWindow. This class cannot be inherited.
/// </summary>
public sealed partial class ThemeWindow : ISynergy.Framework.UI.Controls.Window, IThemeWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeWindow"/> class.
    /// </summary>
    public ThemeWindow()
    {
        InitializeComponent();

        var languageService = Application.Services.GetRequiredService<ILanguageService>();
        PrimaryButtonText = languageService.GetString("Save");
        SecondaryButtonText = languageService.GetString("Cancel");
    }
}
