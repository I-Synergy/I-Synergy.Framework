using Microsoft.AspNetCore.Components;
using Sample.ViewModels;
using System.ComponentModel;

namespace Sample.Components.Pages;

public partial class CommandDemoView
{
    [Inject]
    private CommandDemoViewModel? ViewModel { get; set; }

    protected override void OnInitialized()
    {
        if (ViewModel is not null)
        {
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.IncrementCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
            ViewModel.DecrementCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
            ViewModel.ResetCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.StateHasChanged();
    }

    private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
    {
        base.StateHasChanged();
    }

    public void Dispose()
    {
        if (ViewModel is not null)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            ViewModel.IncrementCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
            ViewModel.DecrementCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
            ViewModel.ResetCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
        }
    }
}