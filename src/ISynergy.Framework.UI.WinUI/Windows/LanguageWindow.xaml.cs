using ISynergy.Framework.Core.Services;
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

        PrimaryButtonText = LanguageService.Default.GetString("Ok");
        SecondaryButtonText = LanguageService.Default.GetString("Cancel");
    }
}