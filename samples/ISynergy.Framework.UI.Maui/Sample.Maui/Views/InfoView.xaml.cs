using ISynergy.Framework.Core.Abstractions;
using Sample.ViewModels;

namespace Sample.Views;

public partial class InfoView
{
    public InfoView(IContext context)
        : base(context, typeof(InfoViewModel))
    {
        InitializeComponent();
    }
}