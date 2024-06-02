using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.Abstractions;

namespace Sample.Views;

[Scoped(true)]
public partial class SettingsView : IView
{
    public SettingsView()
    {
        InitializeComponent();
    }
}