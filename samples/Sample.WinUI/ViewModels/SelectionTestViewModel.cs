using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
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
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return BaseCommonServices.LanguageService.GetString("Converters"); } }

    /// <summary>
    /// Gets or sets the select single command.
    /// </summary>
    /// <value>The select single command.</value>
    public AsyncRelayCommand SelectSingleCommand { get; private set; }
    /// <summary>
    /// Gets or sets the select multiple command.
    /// </summary>
    /// <value>The select multiple command.</value>
    public AsyncRelayCommand SelectMultipleCommand { get; private set; }

    /// <summary>
    /// Show Yes/No dialog.
    /// </summary>
    public AsyncRelayCommand ShowDialogYesNo { get; set; }

    /// <summary>
    /// Show Yes/No/Cancel dialog.
    /// </summary>
    public AsyncRelayCommand ShowDialogYesNoCancel { get; set; }

    /// <summary>
    /// Show Ok dialog.
    /// </summary>
    public AsyncRelayCommand ShowDialogOk { get; set; }

    /// <summary>
    /// Show Ok/Cancel dialog.
    /// </summary>
    public AsyncRelayCommand ShowDialogOkCancel { get; set; }

    public AsyncRelayCommand ShowUnitsCommand { get; private set; }
    public AsyncRelayCommand ShowTestCommand { get; private set; }

    /// <summary>
    /// Gets or sets the selected test items.
    /// </summary>
    /// <value>The selected test items.</value>
    public ObservableCollection<TestItem> SelectedTestItems { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionTestViewModel"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    public SelectionTestViewModel(
        IContext context,
        ICommonServices commonServices,
        ILogger logger)
        : base(context, commonServices, logger)
    {
        SelectSingleCommand = new AsyncRelayCommand(SelectSingleAsync);
        SelectMultipleCommand = new AsyncRelayCommand(SelectMultipleAsync);
        ShowDialogYesNo = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.YesNo));
        ShowDialogYesNoCancel = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.YesNoCancel));
        ShowDialogOk = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.OK));
        ShowDialogOkCancel = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.OKCancel));
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
        TestViewModel vm = new TestViewModel(Context, BaseCommonServices, Logger);
        vm.Submitted += Vm_Submitted;
        await BaseCommonServices.DialogService.ShowDialogAsync(typeof(TestWindow), vm);
    }

    private async void Vm_Submitted(object sender, SubmitEventArgs<object> e)
    {
        if (sender is TestViewModel vm)
            vm.Submitted -= Vm_Submitted;

        CanExecuteTest = !CanExecuteTest;

        await BaseCommonServices.DialogService.ShowInformationAsync($"{e.Result} selected.");
    }

    public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName.Equals(nameof(CanExecuteTest)))
        {
            ShowTestCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task ShowDialogAsync(MessageBoxButton buttons)
    {
        MessageBoxResult result = await BaseCommonServices.DialogService.ShowMessageAsync(
                            $"Testing {buttons} Dialog",
                            "Test",
                            buttons);

        await BaseCommonServices.DialogService.ShowInformationAsync($"{result} selected.", "Result...");
    }

    /// <summary>
    /// Selects the multiple asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task SelectMultipleAsync()
    {
        ViewModelSelectionBlade<TestItem> selectionVm = new ViewModelSelectionBlade<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Multiple);
        selectionVm.Submitted += SelectionVm_MultipleSubmitted;
        return BaseCommonServices.NavigationService.OpenBladeAsync(this, selectionVm);
    }

    /// <summary>
    /// Selects the single asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task SelectSingleAsync()
    {
        ViewModelSelectionBlade<TestItem> selectionVm = new ViewModelSelectionBlade<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Single);
        selectionVm.Submitted += SelectionVm_SingleSubmitted;
        return BaseCommonServices.NavigationService.OpenBladeAsync(this, selectionVm);
    }

    /// <summary>
    /// Selections the vm multiple submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void SelectionVm_MultipleSubmitted(object sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionBlade<TestItem> vm)
            vm.Submitted -= SelectionVm_MultipleSubmitted;

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
        if (sender is ViewModelSelectionBlade<TestItem> vm)
            vm.Submitted -= SelectionVm_SingleSubmitted;

        await BaseCommonServices.DialogService.ShowInformationAsync($"{e.Result.Single().Description} selected.");
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
    public override Task<List<TestItem>> RetrieveItemsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<TestItem>()
        {
            new TestItem { Id = 1, Description = "Test 1"},
            new TestItem { Id = 2, Description = "Test 2"},
            new TestItem { Id = 3, Description = "Test 3"},
            new TestItem { Id = 4, Description = "Test 4"},
            new TestItem { Id = 5, Description = "Test 5"}
        });
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

    public override void Cleanup()
    {
        base.Cleanup();

        SelectedTestItems?.Clear();

        SelectSingleCommand?.Cancel();
        SelectSingleCommand = null;

        SelectMultipleCommand?.Cancel();
        SelectMultipleCommand = null;

        ShowDialogYesNo?.Cancel();
        ShowDialogYesNo = null;

        ShowDialogYesNoCancel?.Cancel();
        ShowDialogYesNoCancel = null;

        ShowDialogOk?.Cancel();
        ShowDialogOk = null;

        ShowDialogOkCancel?.Cancel();
        ShowDialogOkCancel = null;

        ShowUnitsCommand?.Cancel();
        ShowUnitsCommand = null;

        ShowTestCommand?.Cancel();
        ShowTestCommand = null;
    }
}
