using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using Microsoft.Extensions.DependencyInjection;

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
        var languageService = Application.Services.GetRequiredService<ILanguageService>();
        PrimaryButtonText = languageService.GetString("Ok");
        SecondaryButtonText = languageService.GetString("Cancel");
    }
}