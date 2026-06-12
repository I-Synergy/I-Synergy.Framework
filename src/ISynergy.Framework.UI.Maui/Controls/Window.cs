using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Controls;

[Lifetime(Lifetimes.Scoped)]
public class Window : ContentPage, IWindow
{
    public IViewModel ViewModel
    {
        get
        {
            if (BindingContext is IViewModel viewModel)
                return viewModel;

            return default!;
        }
        set
        {
            if (BindingContext is IViewModel oldVm)
                oldVm.Closed -= OnViewModelClosed;

            BindingContext = value;

            if (value is not null)
                value.Closed += OnViewModelClosed;

            SetBinding(View.TitleProperty, new Binding(nameof(value.Title)));
        }
    }

    private bool _isViewModelClosed;

    private void OnViewModelClosed(object? sender, EventArgs e)
    {
        _isViewModelClosed = true;

        // Detach the visual tree and break the BindingContext reference to allow
        // the ViewModel to be GC'd even if MAUI retains the ContentPage after
        // PopModalAsync. Setting Content to null releases all child bindings,
        // including runtime bindings (from x:DataType="{x:Null}") that may cache
        // intermediate path objects like SelectedItem.
        Content = null;
        BindingContext = null;
    }

    public Window()
    {
        Background = new SolidColorBrush(Colors.Transparent);
    }

    /// <summary>
    /// When the dialog window appears, automatically inherits the <see cref="FlowDirection"/>
    /// from the application's main <see cref="Microsoft.Maui.Controls.Window"/>.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Application.Current?.Windows.Count > 0)
        {
            var mainWindow = Application.Current.Windows[0];
            if (mainWindow.FlowDirection != FlowDirection.MatchParent)
            {
                FlowDirection = mainWindow.FlowDirection;
            }
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        if (!_isViewModelClosed && ViewModel is { IsDisposed: false } vm)
            _ = vm.CloseAsync();

        // Always clear BindingContext when navigating away — releases the
        // ViewModel reference even if CloseAsync was already called through
        // another path (e.g. platform-initiated dismissal).
        if (BindingContext is not null)
            BindingContext = null;
    }

    protected override bool OnBackButtonPressed()
    {
        if (ViewModel is { IsDisposed: false } vm)
        {
            _ = vm.CloseAsync();
            return true;
        }
        return base.OnBackButtonPressed();
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
            // Detach the visual tree and clear BindingContext to break all reference
            // chains to the ViewModel. This allows the ViewModel to be GC'd even if
            // MAUI retains the ContentPage after PopModalAsync. The ViewModel itself
            // is disposed by the DI scope that created it — we only remove our strong
            // references here.
            Content = null;
            BindingContext = null;
        }
    }

    public Task CloseAsync() => throw new NotSupportedException("Close is not supported on the .Net Maui platform.");
    #endregion
}
