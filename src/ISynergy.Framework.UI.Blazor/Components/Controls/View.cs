using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Components.Controls;

[Bindable(true)]
[Lifetime(Lifetimes.Scoped)]
public partial class View<TViewModel> : ComponentBase, IView
    where TViewModel : class, IViewModel
{
    private TViewModel? _viewModel;
    private bool _isEnabled = true;

    /// <summary>
    /// Gets or sets the viewmodel and data context for a view.
    /// </summary>
    /// <value>The data context.</value>
    public TViewModel? ViewModel
    {
        get => _viewModel;
        set => _viewModel = value;
    }

    IViewModel? IView.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value is null ? null : (TViewModel)value;
    }

    /// <summary>
    /// Gets or sets the IsEnabled property value.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

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
