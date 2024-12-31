using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;

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

        PrimaryButtonText = LanguageService.Default.GetString("Ok");
        SecondaryButtonText = LanguageService.Default.GetString("Cancel");
    }
}
