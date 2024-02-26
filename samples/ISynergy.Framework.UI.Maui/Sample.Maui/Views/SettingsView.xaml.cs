using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using Sample.ViewModels;

namespace Sample.Views;

[Singleton(true)]
public partial class SettingsView : IView
{
	public SettingsView(IContext context, SettingsViewModel viewModel)
       : base(context, viewModel)
    {
		InitializeComponent();
	}
}