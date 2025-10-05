using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace ISynergy.Framework.AspNetCore.Blazor.Components.Controls;

[Bindable(true)]
[Lifetime(Lifetimes.Scoped)]
public partial class View<TViewModel> : ComponentBase, IView
    where TViewModel : class, IViewModel
{
    /// <summary>
    /// Gets or sets the viewmodel and data context for a view.
    /// </summary>
    /// <value>The data context.</value>
    [Inject] public TViewModel? ViewModel { get; set; }

    /// <summary>
    /// Gets or sets the IsEnabled property value.
    /// </summary>
    [Parameter] public bool IsEnabled { get; set; }

    /// <summary>
    /// Collection of commands that have been subscribed to for CanExecuteChanged events.
    /// </summary>
    private readonly List<ICommand> _subscribedCommands = new();

    IViewModel? IView.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value is null ? null : (TViewModel)value;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (ViewModel is not null)
        {
            if (!ViewModel.IsInitialized)
                await ViewModel.InitializeAsync();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;

            SubscribeToViewModelCommands();
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        StateHasChanged();
    }

    /// <summary>
    /// Automatically subscribes to CanExecuteChanged events for all ICommand properties in the ViewModel.
    /// </summary>
    private void SubscribeToViewModelCommands()
    {
        if (ViewModel is null)
            return;

        var commandProperties = ViewModel.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(ICommand).IsAssignableFrom(p.PropertyType))
            .ToList();

        foreach (var property in commandProperties)
        {
            if (property.GetValue(ViewModel) is ICommand command)
            {
                command.CanExecuteChanged += OnCommandCanExecuteChanged;
                _subscribedCommands.Add(command);
            }
        }
    }

    /// <summary>
    /// Unsubscribes from CanExecuteChanged events for all subscribed commands.
    /// </summary>
    private void UnsubscribeFromViewModelCommands()
    {
        foreach (var command in _subscribedCommands)
        {
            command.CanExecuteChanged -= OnCommandCanExecuteChanged;
        }

        _subscribedCommands.Clear();
    }

    /// <summary>
    /// Handles CanExecuteChanged events from commands and triggers a UI refresh.
    /// </summary>
    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
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
                UnsubscribeFromViewModelCommands();
                ViewModel.Dispose();
            }
        }

        // free native resources if there are any.
    }
    #endregion
}
