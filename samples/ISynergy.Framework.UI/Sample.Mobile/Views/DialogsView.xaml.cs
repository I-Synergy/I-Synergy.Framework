using ISynergy.Framework.Core.Abstractions;
using Sample.ViewModels;

namespace Sample.Views;

public partial class DialogsView
{
    public DialogsView(IContext context)
        : base(context, typeof(DialogsViewModel))
    {
        InitializeComponent();
    }
}