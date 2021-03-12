using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
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

        public Command Select_Command { get; set; }

        public ObservableCollection<TestItem> SelectedTestItems { get; set; }

        public SelectionTestViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            Select_Command = new Command(async () => await SelectAsync());
        }

        private Task SelectAsync()
        {
            var selectionVm = new SelectionViewModel<TestItem>(Context, BaseCommonServices, _loggerFactory, Items, SelectedTestItems, SelectionModes.Single);
            selectionVm.Submitted += SelectionVm_Submitted;
            return BaseCommonServices.NavigationService.OpenBladeAsync(this, selectionVm);
        }

        private void SelectionVm_Submitted(object sender, ISynergy.Framework.Mvvm.Events.SubmitEventArgs<ObservableCollection<TestItem>> e)
        {
            if (sender is SelectionViewModel<TestItem> vm)
                vm.Submitted -= SelectionVm_Submitted;

            SelectedTestItems = new ObservableCollection<TestItem>(e.Result.Cast<TestItem>());
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
