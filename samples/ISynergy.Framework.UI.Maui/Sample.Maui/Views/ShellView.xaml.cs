using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using Sample.ViewModels;

namespace Sample.Views;

public partial class ShellView : Shell, IShellView
{
    public IViewModel ViewModel
    {
        get => BindingContext is IViewModel viewModel ? viewModel : null;
        set => BindingContext = value;
    }

    public ShellView()
    {
        InitializeComponent();
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (ViewModel is ShellViewModel viewModel && viewModel.Context.IsAuthenticated)
        {
            if (viewModel.PrimaryItems.Count > 0 && viewModel.PrimaryItems.First() is NavigationItem navigationItem && navigationItem.Command.CanExecute(navigationItem.CommandParameter))
                navigationItem.Command.Execute(navigationItem.CommandParameter);
        }
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

    // NOTE: Leave out the finalizer altogether if this class doesn't
    // own unmanaged resources, but leave the other methods
    // exactly as they are.
    //~ObservableClass()
    //{
    //    // Finalizer calls Dispose(false)
    //    Dispose(false);
    //}

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