using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using Sample.ViewModels;

namespace Sample.Views;

[Lifetime(Lifetimes.Scoped)]
public partial class HelpView : ContentPage, IView
{
    public IViewModel ViewModel
    {
        get => (IViewModel)BindingContext;
        set => BindingContext = value;
    }

    public HelpView(HelpViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }

    #region IDisposable
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ViewModel?.Dispose();
        }
    }
    #endregion
}
