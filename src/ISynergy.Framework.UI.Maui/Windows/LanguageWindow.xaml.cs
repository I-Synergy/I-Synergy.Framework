using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.Abstractions.Windows;

namespace ISynergy.Framework.UI.Windows;

[Scoped(true)]
public partial class LanguageWindow : ILanguageWindow
{
    public LanguageWindow()
    {
        InitializeComponent();
    }
}