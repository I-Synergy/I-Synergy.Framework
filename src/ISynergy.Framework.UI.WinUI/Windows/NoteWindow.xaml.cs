using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class NoteWindow. This class cannot be inherited.
/// </summary>
public sealed partial class NoteWindow : ISynergy.Framework.UI.Controls.Window, INoteWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoteWindow"/> class.
    /// </summary>
    public NoteWindow()
    {
        InitializeComponent();

        var languageService = Application.Services.GetRequiredService<ILanguageService>();
        PrimaryButtonText = languageService.GetString("Ok");
        SecondaryButtonText = languageService.GetString("Cancel");
    }
}
