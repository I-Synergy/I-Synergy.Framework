using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;

namespace Sample.Views;

[Lifetime(Lifetimes.Scoped)]
public partial class SettingsView : IView
{
    public SettingsView()
    {
        InitializeComponent();
    }
}