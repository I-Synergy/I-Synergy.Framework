using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;
using Sample.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Sample.ViewModels;

/// <summary>
/// Class SelectionTestViewModel.
/// </summary>
public class SelectionTestViewModel : ViewModelBladeView<TestItem>
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return LanguageService.Default.GetString("Converters"); } }

    public AsyncRelayCommand SelectSingleCommand { get; private set; }
    public AsyncRelayCommand SelectMultipleCommand { get; private set; }
    public AsyncRelayCommand ShowDialogYesNo { get; set; }
    public AsyncRelayCommand ShowDialogYesNoCancel { get; set; }
    public AsyncRelayCommand ShowDialogOk { get; set; }
    public AsyncRelayCommand ShowDialogOkCancel { get; set; }
    public AsyncRelayCommand ShowUnitsCommand { get; private set; }
    public AsyncRelayCommand ShowTestCommand { get; private set; }

    /// <summary>
    /// Gets or sets the selected test items.
    /// </summary>
    /// <value>The selected test items.</value>
    public ObservableCollection<TestItem> SelectedTestItems { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionTestViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="dialogService"></param>
    /// <param name="navigationService"></param>
    /// <param name="logger"></param>
    public SelectionTestViewModel(ICommonServices commonServices, IDialogService dialogService, INavigationService navigationService, ILogger<SelectionTestViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;

        SelectSingleCommand = new AsyncRelayCommand(SelectSingleAsync);
        SelectMultipleCommand = new AsyncRelayCommand(SelectMultipleAsync);
        ShowDialogYesNo = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButtons.YesNo));
        ShowDialogYesNoCancel = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButtons.YesNoCancel));
        ShowDialogOk = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButtons.OK));
        ShowDialogOkCancel = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButtons.OKCancel));
        ShowUnitsCommand = new AsyncRelayCommand(ShowUnitsAsync);
        ShowTestCommand = new AsyncRelayCommand(ShowUnitsAsync, canExecute: () => CanExecuteTest);
    }

    /// <summary>
    /// Gets or sets the CanExecuteTest property value.
    /// </summary>
    public bool CanExecuteTest
    {
        get => GetValue<bool>();
        private set => SetValue(value);
    }


    private async Task ShowUnitsAsync()
    {
        TestViewModel vm = _commonServices.ScopedContextService.GetRequiredService<TestViewModel>();
        vm.Submitted += Vm_Submitted;
        await _dialogService.ShowDialogAsync(typeof(TestWindow), vm);
    }

    private async void Vm_Submitted(object? sender, SubmitEventArgs<object> e)
    {
        if (sender is TestViewModel vm)
            vm.Submitted -= Vm_Submitted;

        CanExecuteTest = !CanExecuteTest;

        await _dialogService.ShowInformationAsync($"{e.Result} selected.");
    }

    public override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName!.Equals(nameof(CanExecuteTest)))
        {
            ShowTestCommand?.NotifyCanExecuteChanged();
        }
    }

    private async Task ShowDialogAsync(MessageBoxButtons buttons)
    {
        MessageBoxResult result = await _dialogService.ShowMessageAsync(
                            $"Testing {buttons} Dialog",
                            "Test",
                            buttons);

        await _dialogService.ShowInformationAsync($"{result} selected.", "Result...");
    }

    /// <summary>
    /// Selects the multiple asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task SelectMultipleAsync()
    {
        ViewModelSelectionBlade<TestItem> selectionVm = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestItem>>();
        selectionVm.SetSelectionMode(SelectionModes.Multiple);
        selectionVm.SetItems(Items);
        selectionVm.SetSelectedItems(SelectedTestItems);

        selectionVm.Submitted += SelectionVm_MultipleSubmitted;
        return _navigationService.OpenBladeAsync(this, selectionVm);
    }

    /// <summary>
    /// Selects the single asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task SelectSingleAsync()
    {
        ViewModelSelectionBlade<TestItem> selectionVm = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestItem>>();
        selectionVm.SetSelectionMode(SelectionModes.Single);
        selectionVm.SetItems(Items);
        selectionVm.SetSelectedItems(SelectedTestItems);
        selectionVm.Submitted += SelectionVm_SingleSubmitted;
        return _navigationService.OpenBladeAsync(this, selectionVm);
    }

    /// <summary>
    /// Selections the vm multiple submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void SelectionVm_MultipleSubmitted(object? sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionBlade<TestItem> vm)
            vm.Submitted -= SelectionVm_MultipleSubmitted;

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
        if (sender is ViewModelSelectionBlade<TestItem> vm)
            vm.Submitted -= SelectionVm_SingleSubmitted;

        await _dialogService.ShowInformationAsync($"{e.Result.Single().Description} selected.");
    }

    /// <summary>
    /// Adds the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Task AddAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Edits the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Task EditAsync(TestItem e)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Task RemoveAsync(TestItem e)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves the items asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;TestItem&gt;&gt;.</returns>
    public override Task RetrieveItemsAsync(CancellationToken cancellationToken)
    {
        Items.AddNewRange(new List<TestItem>()
        {
            new TestItem { Id = 1, Description = "Test 1"},
            new TestItem { Id = 2, Description = "Test 2"},
            new TestItem { Id = 3, Description = "Test 3"},
            new TestItem { Id = 4, Description = "Test 4"},
            new TestItem { Id = 5, Description = "Test 5"}
        });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Searches the asynchronous.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <returns>Task.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Task SearchAsync(object e)
    {
        throw new NotImplementedException();
    }

    public override void Cleanup(bool isClosing = true)
    {
        SelectedTestItems?.Clear();
        base.Cleanup(isClosing);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SelectSingleCommand?.Dispose();
            SelectMultipleCommand?.Dispose();
            ShowDialogYesNo?.Dispose();
            ShowDialogYesNoCancel?.Dispose();
            ShowDialogOk?.Dispose();
            ShowDialogOkCancel?.Dispose();
            ShowUnitsCommand?.Dispose();
            ShowTestCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
