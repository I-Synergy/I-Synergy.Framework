using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

[Scoped(true)]
public class ControlsViewModel : ViewModelNavigation<object>
{
    public override string Title { get { return BaseCommonServices.LanguageService.GetString("Controls"); } }

    public ObservableCollection<TestItem> Items { get; set; }
    public ObservableCollection<TestItem> SelectedTestItems { get; set; }

    public RelayCommand BusyOnCommand { get; private set; }
    public RelayCommand NotImplementedErrorCommand { get; private set; }
    public AsyncRelayCommand AsyncNotImplementedErrorCommand { get; private set; }
    public AsyncRelayCommand ShowMemoCommand { get; private set; }
    public AsyncRelayCommand SelectSingleCommand { get; private set; }
    public AsyncRelayCommand SelectMultipleCommand { get; private set; }
    public AsyncRelayCommand<TestItem> NavigateToDetailCommand { get; private set; }

    public ControlsViewModel(IContext context, IBaseCommonServices commonServices, ILogger logger, bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        BusyOnCommand = new RelayCommand(StartTimer);
        ShowMemoCommand = new AsyncRelayCommand(ShowMemoAsync);
        SelectSingleCommand = new AsyncRelayCommand(SelectSingleAsync);
        SelectMultipleCommand = new AsyncRelayCommand(SelectMultipleAsync);
        NavigateToDetailCommand = new AsyncRelayCommand<TestItem>(NavigateToDetailAsync);
        NotImplementedErrorCommand = new RelayCommand(() => throw new NotImplementedException());
        AsyncNotImplementedErrorCommand = new AsyncRelayCommand(() => throw new NotImplementedException());

        Items =
        [
            new TestItem { Id = 1, Description = "Test 1"},
            new TestItem { Id = 2, Description = "Test 2"},
            new TestItem { Id = 3, Description = "Test 3"},
            new TestItem { Id = 4, Description = "Test 4"},
            new TestItem { Id = 5, Description = "Test 5"}
        ];
    }

    private async Task NavigateToDetailAsync(TestItem item)
    {
        var detailsVm = new DetailsViewModel(Context, BaseCommonServices, Logger);
        await BaseCommonServices.NavigationService.NavigateAsync(detailsVm);
    }

    /// <summary>
    /// Selects the multiple asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SelectMultipleAsync()
    {
        var selectionVm = new ViewModelSelectionDialog<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Multiple);
        selectionVm.Submitted += SelectionVm_MultipleSubmitted;
        await BaseCommonServices.DialogService.ShowDialogAsync(typeof(ISelectionWindow), selectionVm);
    }

    /// <summary>
    /// Selects the single asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SelectSingleAsync()
    {
        var selectionVm = new ViewModelSelectionDialog<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Single);
        selectionVm.Submitted += SelectionVm_SingleSubmitted;
        await BaseCommonServices.DialogService.ShowDialogAsync(typeof(ISelectionWindow), selectionVm);
    }

    /// <summary>
    /// Selections the vm multiple submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void SelectionVm_MultipleSubmitted(object sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionDialog<TestItem> vm)
            vm.Submitted -= SelectionVm_SingleSubmitted;

        SelectedTestItems = new ObservableCollection<TestItem>();
        SelectedTestItems.AddRange(e.Result);

        await BaseCommonServices.DialogService.ShowInformationAsync($"{string.Join(", ", e.Result.Select(s => s.Description))} selected.");
    }

    /// <summary>
    /// Selections the vm single submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void SelectionVm_SingleSubmitted(object sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionDialog<TestItem> vm)
            vm.Submitted -= SelectionVm_SingleSubmitted;

        await BaseCommonServices.DialogService.ShowInformationAsync($"{e.Result.Single().Description} selected.");
    }

    private async Task ShowMemoAsync()
    {
        var noteVM = new NoteViewModel(Context, BaseCommonServices, Logger, "");
        noteVM.Submitted += NoteVM_Submitted;
        await BaseCommonServices.DialogService.ShowDialogAsync(typeof(INoteWindow), noteVM);
    }

    private async void NoteVM_Submitted(object sender, SubmitEventArgs<string> e)
    {
        if (sender is NoteViewModel vm)
            vm.Submitted -= NoteVM_Submitted;

        await BaseCommonServices.DialogService.ShowInformationAsync(e.Result);
    }

    private System.Timers.Timer _timer;

    private void StartTimer()
    {
        _timer = new(5000);
        _timer.Elapsed += Timer_Elapsed;
        BaseCommonServices.BusyService.StartBusy();
        _timer.Enabled = true;
        _timer.AutoReset = true;
        _timer.Start();
    }

    private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        BaseCommonServices.BusyService.EndBusy();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            if (_timer is not null)
            {
                _timer.Stop();
                _timer.Elapsed -= Timer_Elapsed;
                _timer.Dispose();
            }
        }
    }
}
