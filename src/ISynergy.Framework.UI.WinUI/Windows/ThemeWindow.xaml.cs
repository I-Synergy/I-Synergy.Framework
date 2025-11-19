using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.UI.Abstractions.Windows;

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

        var languageService = ServiceLocator.Default.GetRequiredService<ILanguageService>();
        PrimaryButtonText = languageService.GetString("Save");
        SecondaryButtonText = languageService.GetString("Cancel");
    }
}
