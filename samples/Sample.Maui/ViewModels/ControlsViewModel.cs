using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Windows;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

[Lifetime(Lifetimes.Scoped)]
public class ControlsViewModel : ViewModelNavigation<object>
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private System.Timers.Timer? _timer;

    public override string Title { get { return _commonServices.LanguageService.GetString("Controls"); } }

    public ObservableCollection<TestItem> Items { get; set; } = new();
    public ObservableCollection<TestItem> SelectedTestItems { get; set; } = new();

    /// <summary>
    /// Gets or sets the File property value.
    /// </summary>
    public byte[] File
    {
        get => GetValue<byte[]>();
        set => SetValue(value);
    }

    public RelayCommand BusyOnCommand { get; private set; }
    public RelayCommand NotImplementedErrorCommand { get; private set; }
    public AsyncRelayCommand AsyncNotImplementedErrorCommand { get; private set; }
    public AsyncRelayCommand ShowMemoCommand { get; private set; }
    public AsyncRelayCommand SelectSingleCommand { get; private set; }
    public AsyncRelayCommand SelectMultipleCommand { get; private set; }
    public AsyncRelayCommand<TestItem> NavigateToDetailCommand { get; private set; }
    public AsyncRelayCommand ShowErrorWindowCommand { get; private set; }

    public ControlsViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ILogger<ControlsViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;

        BusyOnCommand = new RelayCommand(StartTimer);
        ShowMemoCommand = new AsyncRelayCommand(ShowMemoAsync);
        SelectSingleCommand = new AsyncRelayCommand(SelectSingleAsync);
        SelectMultipleCommand = new AsyncRelayCommand(SelectMultipleAsync);
        NavigateToDetailCommand = new AsyncRelayCommand<TestItem>(NavigateToDetailAsync);
        NotImplementedErrorCommand = new RelayCommand(() => throw new NotImplementedException());
        AsyncNotImplementedErrorCommand = new AsyncRelayCommand(() => throw new NotImplementedException());
        ShowErrorWindowCommand = new AsyncRelayCommand(ShowErrorWindowAsync);

        Items =
        [
            new TestItem { Id = 1, Description = "Test 1"},
            new TestItem { Id = 2, Description = "Test 2"},
            new TestItem { Id = 3, Description = "Test 3"},
            new TestItem { Id = 4, Description = "Test 4"},
            new TestItem { Id = 5, Description = "Test 5"}
        ];
    }

    private async Task ShowErrorWindowAsync()
    {
        var testVm = _commonServices.ScopedContextService.GetRequiredService<TestExceptionViewModel>();
        testVm.Submitted += TestVm_Submitted;
        await _dialogService.ShowDialogAsync(typeof(ITestExceptionWindow), testVm);
    }

    private void TestVm_Submitted(object? sender, SubmitEventArgs<object> e)
    {
    }

    private async Task NavigateToDetailAsync(TestItem item)
    {
        var detailsVm = _commonServices.ScopedContextService.GetRequiredService<DetailsViewModel>();
        await _navigationService.NavigateAsync(detailsVm);
    }

    /// <summary>
    /// Selects the multiple asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SelectMultipleAsync()
    {
        var selectionVm = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionDialog<TestItem>>();
        selectionVm.SetSelectionMode(SelectionModes.Multiple);
        selectionVm.SetItems(Items);
        selectionVm.SetSelectedItems(SelectedTestItems);
        selectionVm.Submitted += SelectionVm_MultipleSubmitted;
        await _dialogService.ShowDialogAsync(typeof(ISelectionWindow), selectionVm);
    }

    /// <summary>
    /// Selects the single asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SelectSingleAsync()
    {
        var selectionVm = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionDialog<TestItem>>();
        selectionVm.SetSelectionMode(SelectionModes.Single);
        selectionVm.SetItems(Items);
        selectionVm.SetSelectedItems(SelectedTestItems);
        selectionVm.Submitted += SelectionVm_SingleSubmitted;
        await _dialogService.ShowDialogAsync(typeof(ISelectionWindow), selectionVm);
    }

    /// <summary>
    /// Selections the vm multiple submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void SelectionVm_MultipleSubmitted(object? sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionDialog<TestItem> vm)
            vm.Submitted -= SelectionVm_SingleSubmitted;

        SelectedTestItems = new ObservableCollection<TestItem>();
        SelectedTestItems.AddRange(e.Result);

        await _dialogService.ShowInformationAsync($"{string.Join(", ", e.Result.Select(s => s.Description))} selected.");
    }

    /// <summary>
    /// Selections the vm single submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void SelectionVm_SingleSubmitted(object? sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionDialog<TestItem> vm)
            vm.Submitted -= SelectionVm_SingleSubmitted;

        await _dialogService.ShowInformationAsync($"{e.Result.Single().Description} selected.");
    }

    private async Task ShowMemoAsync()
    {
        var noteVM = _commonServices.ScopedContextService.GetRequiredService<NoteViewModel>();
        noteVM.Submitted += NoteVM_Submitted;
        await _dialogService.ShowDialogAsync(typeof(INoteWindow), noteVM);
    }

    private async void NoteVM_Submitted(object? sender, SubmitEventArgs<string> e)
    {
        if (sender is NoteViewModel vm)
            vm.Submitted -= NoteVM_Submitted;

        await _dialogService.ShowInformationAsync(e.Result);
    }

    private void StartTimer()
    {
        _timer = new(5000);
        _timer.Elapsed += Timer_Elapsed;
        _commonServices.BusyService.StartBusy();
        _timer.Enabled = true;
        _timer.AutoReset = true;
        _timer.Start();
    }

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _commonServices.BusyService.StopBusy();
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
