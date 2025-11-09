using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Windows;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class LanguageWindow. This class cannot be inherited.
/// </summary>
public partial class LanguageWindow : ISynergy.Framework.UI.Controls.Window, ILanguageWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageWindow"/> class.
    /// </summary>
    public LanguageWindow()
    {
        InitializeComponent();
        var languageService = ServiceLocator.Default.GetRequiredService<ILanguageService>();
        PrimaryButtonText = languageService.GetString("Ok");
        SecondaryButtonText = languageService.GetString("Cancel");
    }
}