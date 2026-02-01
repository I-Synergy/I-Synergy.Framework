using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.FluentUI.AspNetCore.Components;
using Sample.Components.Dialogs;
using Sample.Models;

namespace Sample.ViewModels;

public class CommandDemoViewModel : ViewModel
{
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Gets or sets the Counter property value.
    /// </summary>
    public int Counter
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    public RelayCommand IncrementCommand { get; }
    public RelayCommand DecrementCommand { get; }
    public RelayCommand ResetCommand { get; }
    public AsyncRelayCommand OpenSampleDialogCommand { get; }

    public CommandDemoViewModel(ICommonServices commonServices, IDialogService dialogService, ILogger<CommandDemoViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;

        IncrementCommand = new RelayCommand(() => Counter++);
        DecrementCommand = new RelayCommand(() => Counter--, () => Counter > 0);
        ResetCommand = new RelayCommand(() => Counter = 0, () => Counter != 0);
        OpenSampleDialogCommand = new AsyncRelayCommand(OpenSampleDialogAsync);
    }

    private async Task OpenSampleDialogAsync()
    {
        var viewModel = _commonServices.ScopedContextService.GetRequiredService<SampleDialogViewModel>();
        viewModel.SetSelectedItem(new Budget(), false);
        viewModel.Submitted += HandleDialog;

        var dialog = await _dialogService.ShowDialogAsync<SampleDialog>(viewModel, new()
        {
            Title = "Open Sample Dialog",
            Width = "600px", // Increased width for the wizard
            Height = "auto", // Set appropriate height
            TrapFocus = true,
            Modal = true,
            PreventScroll = true,
            DialogType = DialogType.Dialog,
            PrimaryAction = null, // Remove OK button
            SecondaryAction = null, // Remove Cancel button
            ShowDismiss = true, // Keep the X button for closing
        });
    }

    private void HandleDialog(object? sender, SubmitEventArgs<Budget> e)
    {
        if (sender is SampleDialogViewModel vm)
            vm.Submitted -= HandleDialog;

        //await RefreshAsync();
    }

    //private async Task HandleDialog(DialogResult result)
    //{
    //    if (result.Cancelled)
    //    {
    //        await Task.Run(() => Debug.WriteLine($"Dialog cancelled"));
    //        return;
    //    }

    //    if (result.Data is not null)
    //    {
    //        var budget = result.Data as Budget;
    //        await Task.Run(() => Debug.WriteLine($"Dialog closed by {budget?.StartingDate} {budget?.EndingDate} ({budget?.CreatedDate})"));
    //        return;
    //    }

    //    await Task.Run(() => Debug.WriteLine($"Dialog closed"));
    //}
}
