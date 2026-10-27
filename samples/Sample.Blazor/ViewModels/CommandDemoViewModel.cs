using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.FluentUI.AspNetCore.Components;
using Sample.Components.Dialogs;
using Sample.Components.Dialogs.Base;
using Sample.Models;

namespace Sample.ViewModels;

public class CommandDemoViewModel : ViewModel
{
    private readonly IDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;

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

    public CommandDemoViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        ILogger<CommandDemoViewModel> logger,
        IServiceProvider serviceProvider)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _serviceProvider = serviceProvider;

        IncrementCommand = new RelayCommand(() => Counter++);
        DecrementCommand = new RelayCommand(() => Counter--, () => Counter > 0);
        ResetCommand = new RelayCommand(() => Counter = 0, () => Counter != 0);
        OpenSampleDialogCommand = new AsyncRelayCommand(OpenSampleDialogAsync);
    }

    private async Task OpenSampleDialogAsync()
    {
        var viewModel = _serviceProvider.GetRequiredService<SampleDialogViewModel>();
        viewModel.SetSelectedItem(new Budget(), false);
        viewModel.Submitted += HandleDialog;

        // v5: DialogParameters was replaced by DialogOptions. TrapFocus, PreventScroll, ShowDismiss
        // and DialogType no longer exist; DialogType.Dialog maps to DialogAlignment.Default (the
        // v5 default) and the dismiss (X) header action is present by default. The OK/Cancel
        // primary/secondary actions are not configured here because SampleDialog renders its own
        // footer through the FluentDialogBody.ActionTemplate slot.
        _ = await _dialogService.ShowDialogAsync<SampleDialog>(options =>
        {
            options.Header.Title = "Open Sample Dialog";
            options.Width = "600px"; // Increased width for the wizard

            // v5: DialogOptions.Height is NOT a height lever for a centred dialog. It is written as an
            // inline `height` on the <fluent-dialog> *host* element, whose box is 0x0 because the panel
            // is a shadow-DOM <dialog part="dialog">. Measured on the live DOM: inline `auto`, no
            // height at all, `fit-content` and a concrete `320px` all leave the panel at the full
            // viewport height (900px). The panel is sized from site.css instead - see the
            // "Dialog panel height" section there.
            options.Modal = true;

            // The dialog component's [Parameter] must be named exactly "Content".
            options.Parameters.Add(
                nameof(BaseModelDialog<Budget, SampleDialogViewModel>.Content),
                viewModel);
        });
    }

    private void HandleDialog(object? sender, SubmitEventArgs<Budget> e)
    {
        if (sender is SampleDialogViewModel vm)
            vm.Submitted -= HandleDialog;
    }
}

