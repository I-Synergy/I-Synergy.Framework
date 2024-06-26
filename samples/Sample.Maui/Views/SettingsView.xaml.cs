using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using Sample.ViewModels;

namespace Sample.Views;

[Scoped(true)]
public partial class SettingsView
{
    public SettingsView(IContext context, SettingsViewModel viewModel)
       : base(context, viewModel)
    {
        InitializeComponent();
    }
}