using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public class SelectionTestViewModel : ViewModelBladeView<TestItem>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Converters"); } }

        public Command SelectSingle_Command { get; set; }
        public Command SelectMultiple_Command { get; set; }

        public ObservableCollection<TestItem> SelectedTestItems { get; set; }

        public SelectionTestViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            SelectSingle_Command = new Command(async () => await SelectSingleAsync());
            SelectMultiple_Command = new Command(async () => await SelectMultipleAsync());
        }

        private Task SelectMultipleAsync()
        {
            var selectionVm = new SelectionViewModel<TestItem>(Context, BaseCommonServices, _loggerFactory, Items, SelectedTestItems, SelectionModes.Multiple);
            selectionVm.Submitted += SelectionVm_MultipleSubmitted;
            return BaseCommonServices.NavigationService.OpenBladeAsync(this, selectionVm);
        }

        private Task SelectSingleAsync()
        {
            var selectionVm = new SelectionViewModel<TestItem>(Context, BaseCommonServices, _loggerFactory, Items, SelectedTestItems, SelectionModes.Single);
            selectionVm.Submitted += SelectionVm_SingleSubmitted;
            return BaseCommonServices.NavigationService.OpenBladeAsync(this, selectionVm);
        }

        private async void SelectionVm_MultipleSubmitted(object sender, SubmitEventArgs<List<object>> e)
        {
            if (sender is SelectionViewModel<TestItem> vm)
                vm.Submitted -= SelectionVm_MultipleSubmitted;

            SelectedTestItems = new ObservableCollection<TestItem>(e.Result.Cast<TestItem>());

            await BaseCommonServices.DialogService.ShowInformationAsync($"{string.Join(", ", e.Result.Cast<TestItem>().Select(s => s.Description))} selected.");
        }

        private async void SelectionVm_SingleSubmitted(object sender, SubmitEventArgs<List<object>> e)
        {
            if (sender is SelectionViewModel<TestItem> vm)
                vm.Submitted -= SelectionVm_SingleSubmitted;

            await BaseCommonServices.DialogService.ShowInformationAsync($"{e.Result.Cast<TestItem>().Single().Description} selected.");
        }

        public override Task AddAsync()
        {
            throw new NotImplementedException();
        }

        public override Task EditAsync(TestItem e)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveAsync(TestItem e)
        {
            throw new NotImplementedException();
        }

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

        public override Task SearchAsync(object e)
        {
            throw new NotImplementedException();
        }
    }
}
