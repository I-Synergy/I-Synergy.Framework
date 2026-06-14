using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
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
    private readonly IBladeService _bladeService;

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return _commonServices.LanguageService.GetString("Converters"); } }

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
    /// <param name="bladeService"></param>
    /// <param name="logger"></param>
    public SelectionTestViewModel(ICommonServices commonServices, IDialogService dialogService, INavigationService navigationService, IBladeService bladeService, ILogger<SelectionTestViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;
        _bladeService = bladeService;

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


    private Task ShowUnitsAsync() =>
        _dialogService.ShowDialogAsync<TestWindow, TestViewModel, object>(
            async result =>
            {
                CanExecuteTest = !CanExecuteTest;
                await _dialogService.ShowInformationAsync($"{result} selected.");
            });

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
    private Task SelectMultipleAsync() =>
        _bladeService.OpenBladeAsync<ViewModelMultipleSelectionBlade<TestItem>, List<TestItem>>(
            this,
            async items =>
            {
                SelectedTestItems = new ObservableCollection<TestItem>();
                SelectedTestItems.AddRange(items);
                await _dialogService.ShowInformationAsync($"{string.Join(", ", items.Select(s => s.Description))} selected.");
            },
            initialization: vm => vm.SetItems(Items!));

    private Task SelectSingleAsync() =>
        _bladeService.OpenBladeAsync<ViewModelSingleSelectionBlade<TestItem>, TestItem>(
            this,
            async item =>
            {
                SelectedTestItems = new ObservableCollection<TestItem> { item };
                await _dialogService.ShowInformationAsync($"{item.Description} selected.");
            },
            initialization: vm => vm.SetItems(Items!));

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
        Items?.AddNewRange(new List<TestItem>()
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

