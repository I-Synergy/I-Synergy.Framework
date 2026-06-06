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
            BindingContext = value;
            SetBinding(View.TitleProperty, new Binding(nameof(value.Title)));
        }
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
            // DO NOT dispose ViewModel here.
            // The Window is resolved from a DI scope that is disposed after the dialog
            // is shown but before it is closed. Disposing the ViewModel prematurely clears
            // its Closed event, preventing the DialogService's close handler from firing.
            // The ViewModel lifecycle is managed by the scope that created it (e.g. in
            // OpenColorsAsync) and by the DialogService's close handler.
        }
    }

    public Task CloseAsync() => throw new NotSupportedException("Close is not supported on the .Net Maui platform.");
    #endregion
}
