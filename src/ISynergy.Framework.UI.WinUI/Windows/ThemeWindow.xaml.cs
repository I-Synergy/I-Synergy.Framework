using ISynergy.Framework.Core.Services;
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

        PrimaryButtonText = LanguageService.Default.GetString("Save");
        SecondaryButtonText = LanguageService.Default.GetString("Cancel");
    }
}
