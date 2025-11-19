using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Scoped)]
public partial class SettingsView
{
    public SettingsView(SettingsViewModel viewModel)
       : base(viewModel)
    {
        InitializeComponent();
    }
}