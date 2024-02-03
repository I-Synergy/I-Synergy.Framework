using ISynergy.Framework.Core.Abstractions;
using Sample.ViewModels;

namespace Sample.Views;

public partial class ValidationView : IView
{
    public ValidationView(IContext context)
        : base(context, typeof(ValidationViewModel))
    {
        InitializeComponent();
    }
}