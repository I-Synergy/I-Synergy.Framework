using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Components.Controls;

[Bindable(true)]
[Lifetime(Lifetimes.Scoped)]
public partial class Window<TModel, TViewModel> : FluentDialog, IWindow
    where TModel : class
    where TViewModel : class, IViewModelDialog<TModel>
{
    private TViewModel? _viewModel;

    [Inject] private ICommonServices _commonServices { get; set; } = default!;

    /// <summary>
    /// Gets or sets the viewmodel and data context for a window.
    /// </summary>
    /// <value>The data context.</value>
    /// <summary>
    /// Gets or sets the viewmodel and data context for a window.
    /// </summary>
    /// <value>The data context.</value>
    [Parameter]
    public TViewModel? ViewModel
    {
        get => _viewModel;
        set
        {
            if (_viewModel != value)
            {
                // Unsubscribe from old ViewModel if it exists
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                    _viewModel.Closed -= OnViewModelClosed;
                }

                _viewModel = value;

                // Subscribe to new ViewModel if it exists
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                    _viewModel.Closed += OnViewModelClosed;
                }
            }
        }
    }

    IViewModel? IWindow.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value is null ? null : (TViewModel)value;
    }

    [Parameter]
    public TViewModel? Content { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Content is TViewModel viewModel)
            ViewModel = viewModel;

        if (ViewModel is not null && !ViewModel.IsInitialized)
            await ViewModel.InitializeAsync();

        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Instance is not null && Instance.Parameters.OnDialogOpened.HasDelegate)
            await Instance.Parameters.OnDialogOpened.InvokeAsync(Instance);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        StateHasChanged();
    }

    private async void OnViewModelClosed(object? sender, EventArgs e)
    {
        await CloseAsync();
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
                ViewModel.Closed -= OnViewModelClosed;
                ViewModel.Dispose();
            }
        }

        // free native resources if there are any.
    }
    #endregion
}
