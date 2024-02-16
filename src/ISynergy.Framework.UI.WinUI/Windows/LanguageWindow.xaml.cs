using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Windows;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class LanguageWindow. This class cannot be inherited.
/// </summary>
[Scoped(true)]
public partial class LanguageWindow : ISynergy.Framework.UI.Controls.Window, ILanguageWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageWindow"/> class.
    /// </summary>
    public LanguageWindow()
    {
        InitializeComponent();

        PrimaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Ok");
        SecondaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Cancel");
    }
}