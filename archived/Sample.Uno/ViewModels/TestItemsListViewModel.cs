﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Enumerations;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

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
    public IEnumerable<TestItem> SelectedItems
    {
        get { return GetValue<IEnumerable<TestItem>>(); }
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
        ClearCommand = new RelayCommand(ClearItems);

        Query = string.Empty;
        Items = [];
    }

    /// <summary>
    /// Gets or sets the clear command.
    /// </summary>
    /// <value>The clear command.</value>
    public RelayCommand ClearCommand { get; private set; }

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
    public override Task SearchAsync(object e) => RefreshAsync();

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
        ViewModelSelectionBlade<TestItem> selectionVM = new ViewModelSelectionBlade<TestItem>(Context, CommonServices, Logger, Items, SelectedItems, ISynergy.Framework.Mvvm.Enumerations.SelectionModes.Single);
        selectionVM.Submitted += SelectionVM_Submitted;
        return CommonServices.NavigationService.OpenBladeAsync<ISelectionView>(this, selectionVM);
    }

    /// <summary>
    /// Selections the vm submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void SelectionVM_Submitted(object sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionBlade<TestItem> vm)
            vm.Submitted -= SelectionVM_Submitted;

        SelectedItems = e.Result;
    }
}
