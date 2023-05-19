using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels
{
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
        public AsyncRelayCommand SelectSingleCommand { get; set; }
        /// <summary>
        /// Gets or sets the select multiple command.
        /// </summary>
        /// <value>The select multiple command.</value>
        public AsyncRelayCommand SelectMultipleCommand { get; set; }

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
        }

        private async Task ShowDialogAsync(MessageBoxButton buttons)
        {
            if (await BaseCommonServices.DialogService.ShowMessageAsync(
                                $"Testing {buttons} Dialog",
                                "Test",
                                buttons) is MessageBoxResult result)
            {
                await BaseCommonServices.DialogService.ShowInformationAsync($"{result} selected.", "Result...");
            }
        }

        /// <summary>
        /// Selects the multiple asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private Task SelectMultipleAsync()
        {
            var selectionVm = new ViewModelSelectionBlade<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Multiple);
            selectionVm.Submitted += SelectionVm_MultipleSubmitted;
            return BaseCommonServices.NavigationService.OpenBladeAsync(this, selectionVm);
        }

        /// <summary>
        /// Selects the single asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private Task SelectSingleAsync()
        {
            var selectionVm = new ViewModelSelectionBlade<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Single);
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

            SelectedTestItems = new ObservableCollection<TestItem>(e.Result);

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
    }
}
