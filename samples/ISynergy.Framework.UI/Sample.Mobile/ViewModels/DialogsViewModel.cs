using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels
{
    public class DialogsViewModel : ViewModelNavigation<object>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Dialogs"); } }

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
        /// Show Ok dialog.
        /// </summary>
        public AsyncRelayCommand ShowDialogOk { get; set; }

        /// <summary>
        /// Show Ok/Cancel dialog.
        /// </summary>
        public AsyncRelayCommand ShowDialogOkCancel { get; set; }

        /// <summary>
        /// Command that shows a note dialog.
        /// </summary>
        public AsyncRelayCommand ShowNoteDialog { get; set; }

        /// <summary>
        /// Gets or sets the selected test items.
        /// </summary>
        /// <value>The selected test items.</value>
        public ObservableCollection<TestItem> SelectedTestItems { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public ObservableCollection<TestItem> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        public DialogsViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            SelectSingleCommand = new AsyncRelayCommand(SelectSingleAsync);
            SelectMultipleCommand = new AsyncRelayCommand(SelectMultipleAsync);
            ShowDialogYesNo = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.YesNo));
            ShowDialogOk = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.OK));
            ShowDialogOkCancel = new AsyncRelayCommand(async () => await ShowDialogAsync(MessageBoxButton.OKCancel));
            ShowNoteDialog = new AsyncRelayCommand(ShowNoteDialogAsync);

            Items = new ObservableCollection<TestItem>()
            {
                new TestItem { Id = 1, Description = "Test 1"},
                new TestItem { Id = 2, Description = "Test 2"},
                new TestItem { Id = 3, Description = "Test 3"},
                new TestItem { Id = 4, Description = "Test 4"},
                new TestItem { Id = 5, Description = "Test 5"},
                new TestItem { Id = 1, Description = "Test 6"},
                new TestItem { Id = 2, Description = "Test 7"},
                new TestItem { Id = 3, Description = "Test 8"},
                new TestItem { Id = 4, Description = "Test 9"},
                new TestItem { Id = 5, Description = "Test 10"},
                new TestItem { Id = 1, Description = "Test 11"},
                new TestItem { Id = 2, Description = "Test 12"},
                new TestItem { Id = 3, Description = "Test 13"},
                new TestItem { Id = 4, Description = "Test 14"},
                new TestItem { Id = 5, Description = "Test 15"}
            };
        }

        private async Task ShowNoteDialogAsync()
        {
            NoteViewModel vm = new(Context, BaseCommonServices, Logger, "Lorem ipsum dolor sit amet");
            vm.Submitted += Vm_Submitted;
            await BaseCommonServices.DialogService.ShowDialogAsync(typeof(INoteWindow), vm);
        }

        private async void Vm_Submitted(object sender, ISynergy.Framework.Mvvm.Events.SubmitEventArgs<string> e)
        {
            if (sender is NoteViewModel vm)
                vm.Submitted -= Vm_Submitted;

            await BaseCommonServices.DialogService.ShowInformationAsync($"'{e.Result}' entered.", "Result...");
        }

        private async Task ShowDialogAsync(MessageBoxButton buttons)
        {
            if (await BaseCommonServices.DialogService.ShowMessageAsync(
                                $"Testing {buttons} Dialog",
                                "Test",
                                buttons) is MessageBoxResult result)
            {
                await BaseCommonServices.DialogService.ShowInformationAsync($"{result} selected.", "Result...");
            };
        }

        /// <summary>
        /// Selects the multiple asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private Task SelectMultipleAsync()
        {
            var selectionVm = new ViewModelSelectionDialog<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Multiple);
            selectionVm.Submitted += SelectionVm_MultipleSubmitted;
            return BaseCommonServices.DialogService.ShowDialogAsync(typeof(ISelectionWindow), selectionVm);
        }

        /// <summary>
        /// Selects the single asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private Task SelectSingleAsync()
        {
            var selectionVm = new ViewModelSelectionDialog<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Single);
            selectionVm.Submitted += SelectionVm_SingleSubmitted;
            return BaseCommonServices.DialogService.ShowDialogAsync(typeof(ISelectionWindow), selectionVm);
        }

        /// <summary>
        /// Selections the vm multiple submitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void SelectionVm_MultipleSubmitted(object sender, SubmitEventArgs<List<TestItem>> e)
        {
            if (sender is ViewModelSelectionDialog<TestItem> vm)
                vm.Submitted -= SelectionVm_MultipleSubmitted;

            SelectedTestItems = new ObservableCollection<TestItem>(e.Result.Cast<TestItem>());

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
    }
}
