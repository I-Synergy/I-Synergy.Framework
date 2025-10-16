using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
using Sample.ViewModels;

namespace Sample.Views;

public partial class ShellView : IShellView
{
    public IViewModel? ViewModel
    {
        get => BindingContext is IViewModel viewModel ? viewModel : null;
        set => BindingContext = value;
    }

    public ShellView(ShellViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;

        if (NavigationExtensions.CreatePage<InfoViewModel>(null) is Page page)
            Detail = new NavigationPage(page);
    }

    #region IDisposable
    // Dispose() calls Dispose(true)
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // free managed resources
            ViewModel?.Dispose();
            ViewModel = null;
        }

        // free native resources if there are any.
    }
    #endregion
}