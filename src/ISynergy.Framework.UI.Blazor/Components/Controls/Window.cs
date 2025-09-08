using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Components.Controls;

[Bindable(true)]
[Lifetime(Lifetimes.Scoped)]
public partial class Window<TViewModel> : FluentDialog, IWindow, IDialogContentComponent<TViewModel?>
    where TViewModel : class, IViewModel
{
    /// <summary>
    /// Gets or sets the viewmodel and data context for a window.
    /// </summary>
    /// <value>The data context.</value>
    public TViewModel? ViewModel
    {
        get
        {
            if (Content is TViewModel viewModel)
                return viewModel;

            return null;
        }
        set
        {
            Content = value;
        }
    }

    IViewModel? IWindow.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value is null ? null : (TViewModel)value;
    }

    public TViewModel? Content { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (ViewModel is not null)
        {
            if (!ViewModel.IsInitialized)
                await ViewModel.InitializeAsync();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        StateHasChanged();
    }

    #region IDisposable
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (ViewModel is not null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.Dispose();
            }
        }

        // free native resources if there are any.
    }
    #endregion
}
