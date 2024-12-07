using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Scoped)]
public partial class SettingsView
{
    public SettingsView(IContext context, SettingsViewModel viewModel)
       : base(context, viewModel)
    {
        InitializeComponent();
    }
}