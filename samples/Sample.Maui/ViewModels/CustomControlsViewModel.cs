using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Windows;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

[Lifetime(Lifetimes.Scoped)]
public class CustomControlsViewModel : ViewModelNavigation<object>
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private System.Timers.Timer? _timer;

    public override string Title { get { return _commonServices.LanguageService.GetString("CustomControls"); } }

    public decimal Price
    {
        get => GetValue<decimal>();
        set => SetValue(value);
    }

    public decimal Weight
    {
        get => GetValue<decimal>();
        set => SetValue(value);
    }

    public int Quantity
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    public string SearchText
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public ObservableCollection<TestItem> Items { get; set; } = new();
    public ObservableCollection<TestItem> SelectedTestItems { get; set; } = new();

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
    public AsyncRelayCommand NavigateToDetailCommand { get; private set; }
    public AsyncRelayCommand ShowErrorWindowCommand { get; private set; }

    public CustomControlsViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ILogger<CustomControlsViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;

        BusyOnCommand = new RelayCommand(StartTimer);
        ShowMemoCommand = new AsyncRelayCommand(ShowMemoAsync);
        SelectSingleCommand = new AsyncRelayCommand(SelectSingleAsync);
        SelectMultipleCommand = new AsyncRelayCommand(SelectMultipleAsync);
        NavigateToDetailCommand = new AsyncRelayCommand(NavigateToDetailAsync);
        NotImplementedErrorCommand = new RelayCommand(() => throw new NotImplementedException());
        AsyncNotImplementedErrorCommand = new AsyncRelayCommand(() => throw new NotImplementedException());
        ShowErrorWindowCommand = new AsyncRelayCommand(ShowErrorWindowAsync);

        SearchText = string.Empty;
        Quantity = 19;
        Weight = 91.19m;
        Price = 19.19m;

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
        var testVm = ServiceLocator.Default.GetRequiredService<TestExceptionViewModel>();
        testVm.Submitted += TestVm_Submitted;
        await _dialogService.ShowDialogAsync(typeof(ITestExceptionWindow), testVm);
    }

    private void TestVm_Submitted(object? sender, SubmitEventArgs<TestItem> e)
    {
    }

    private async Task NavigateToDetailAsync()
    {
        var detailsVm = ServiceLocator.Default.GetRequiredService<DetailsViewModel>();
        await _navigationService.NavigateAsync(detailsVm);
    }

    private Task SelectMultipleAsync()
    {
        var selectionVm = ServiceLocator.Default.GetRequiredService<ViewModelMultipleSelectionDialog<TestItem>>();
        selectionVm.SetItems(Items);
        selectionVm.Submitted += SelectionVm_MultipleSubmitted;
        return _dialogService.ShowDialogAsync(typeof(IMultipleSelectionWindow), selectionVm);
    }

    private Task SelectSingleAsync()
    {
        var selectionVm = ServiceLocator.Default.GetRequiredService<ViewModelSingleSelectionDialog<TestItem>>();
        selectionVm.SetItems(Items);
        selectionVm.Submitted += SelectionVm_SingleSubmitted;
        return _dialogService.ShowDialogAsync(typeof(ISingleSelectionWindow), selectionVm);
    }

    private async void SelectionVm_MultipleSubmitted(object? sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelMultipleSelectionDialog<TestItem> vm)
            vm.Submitted -= SelectionVm_MultipleSubmitted;

        SelectedTestItems = new ObservableCollection<TestItem>();
        SelectedTestItems.AddRange(e.Result);

        await _dialogService.ShowInformationAsync($"{string.Join(", ", e.Result.Select(s => s.Description))} selected.");
    }

    private async void SelectionVm_SingleSubmitted(object? sender, SubmitEventArgs<TestItem> e)
    {
        if (sender is ViewModelSingleSelectionDialog<TestItem> vm)
            vm.Submitted -= SelectionVm_SingleSubmitted;

        await _dialogService.ShowInformationAsync($"{e.Result.Description} selected.");
    }

    private async Task ShowMemoAsync()
    {
        var noteVM = ServiceLocator.Default.GetRequiredService<NoteViewModel>();
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

        if (disposing && _timer is not null)
        {
            _timer.Stop();
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Dispose();
        }
    }
}
