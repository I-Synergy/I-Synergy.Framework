using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Enumerations;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.UI.ViewModels;

namespace Sample.ViewModels
{
    /// <summary>
    /// Class TestItemsListViewModel.
    /// </summary>
    public class TestItemsListViewModel : ViewModelBladeView<TestItem>, IViewModelBladeView
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get
            {
                return "Search example";
            }
        }

        /// <summary>
        /// Gets or sets the Query property value.
        /// </summary>
        /// <value>The query.</value>
        public string Query
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedItems property value.
        /// </summary>
        /// <value>The selected items.</value>
        public List<object> SelectedItems
        {
            get { return GetValue<List<object>>(); }
            set { SetValue(value); }
        }


        /// <summary>
        /// Gets or sets the QueryTypes property value.
        /// </summary>
        public QueryTypes QueryTypes
        {
            get => GetValue<QueryTypes>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the SelectedQueryType property value.
        /// </summary>
        /// <value>The type of the selected query.</value>
        public int SelectedQueryType
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// The common services
        /// </summary>
        protected readonly ICommonServices CommonServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBladeView{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonService">The common service.</param>
        /// <param name="logger">The logger factory.</param>
        public TestItemsListViewModel(IContext context, ICommonServices commonService, ILogger logger)
            : base(context, commonService, logger)
        {
            CommonServices = commonService;

            Submit_Command = new AsyncRelayCommand<TestItem>(SubmitAsync);
            Search_Command = new AsyncRelayCommand<object>(SearchAsync);
            Clear_Command = new RelayCommand(ClearItems);

            Query = string.Empty;
            Items = new ObservableCollection<TestItem>();
        }

        /// <summary>
        /// Gets or sets the clear command.
        /// </summary>
        /// <value>The clear command.</value>
        public RelayCommand Clear_Command { get; set; }

        /// <summary>
        /// The search cancellationtoken
        /// </summary>
        private CancellationTokenSource SearchCancellationtoken = null;

        /// <summary>
        /// Edits the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task EditAsync(TestItem e) => Task.CompletedTask;

        /// <summary>
        /// Retrieves the items asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;TEntity&gt;&gt;.</returns>
        public override Task<List<TestItem>> RetrieveItemsAsync(CancellationToken cancellationToken) => 
            Task.FromResult(new List<TestItem>()
                {
                    new TestItem { Id = 1, Description = "Test 1"},
                    new TestItem { Id = 2, Description = "Test 2"},
                    new TestItem { Id = 3, Description = "Test 3"},
                    new TestItem { Id = 4, Description = "Test 4"},
                    new TestItem { Id = 5, Description = "Test 5"}
                });

        /// <summary>
        /// Removes the asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task RemoveAsync(TestItem e) => Task.CompletedTask;


        /// <summary>
        /// search as an asynchronous operation.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public override async Task SearchAsync(object e)
        {
            SearchCancellationtoken?.Cancel();
            SearchCancellationtoken = new CancellationTokenSource();

            try
            {
                Items = new ObservableCollection<TestItem>(
                    await RetrieveItemsAsync(SearchCancellationtoken.Token));
            }
            catch (OperationCanceledException)
            {
                //Ignore, operation has been canceled
            }
            finally
            {
                SearchCancellationtoken?.Dispose();
                SearchCancellationtoken = null;
            }
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        protected void ClearItems()
        {
            Items.Clear();
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public override Task AddAsync()
        {
            var selectionVM = new SelectionViewModel(Context, CommonServices, Logger, Items, SelectedItems, ISynergy.Framework.Mvvm.Enumerations.SelectionModes.Single);
            selectionVM.Submitted += SelectionVM_Submitted;
            (CommonServices.NavigationService as INavigationServiceExtended)?.OpenBladeAsync(this, selectionVM);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Selections the vm submitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SelectionVM_Submitted(object sender, SubmitEventArgs<List<object>> e)
        {
            SelectedItems = e.Result;
        }
    }
}
