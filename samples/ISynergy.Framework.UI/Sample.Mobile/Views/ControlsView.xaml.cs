using ISynergy.Framework.Core.Abstractions;
using Sample.ViewModels;

namespace Sample.Views;

public partial class ControlsView
{
    public ControlsView(IContext context)
        : base(context, typeof(ControlsViewModel))
    {
        InitializeComponent();
    }
}