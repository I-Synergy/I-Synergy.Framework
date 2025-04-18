﻿using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
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
    public QueryTypes SelectedQueryType
    {
        get { return GetValue<QueryTypes>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Years property value.
    /// </summary>
    /// <value>The years.</value>
    public ObservableCollection<Year> Years
    {
        get { return GetValue<ObservableCollection<Year>>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Selected Year property value.
    /// </summary>
    /// <value>The selected year.</value>
    public Year SelectedYear
    {
        get { return GetValue<Year>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBladeView{TEntity}" /> class.
    /// </summary>
    /// <param name="commonServices">The common service.</param>
    public TestItemsListViewModel(ICommonServices commonServices)
        : base(commonServices)
    {
        ClearCommand = new RelayCommand(ClearItems);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        Query = string.Empty;
        Items = [];

        Years =
        [
         new Year { Value = DateTime.Now.Date.Year, Description = DateTime.Now.Date.Year.ToString() },
          new Year { Value = DateTime.Now.Date.Year - 1, Description = (DateTime.Now.Date.Year - 1).ToString() },
          new Year { Value = DateTime.Now.Date.Year - 2, Description = (DateTime.Now.Date.Year - 2).ToString() },
          new Year { Value = DateTime.Now.Date.Year - 3, Description = (DateTime.Now.Date.Year - 3).ToString() },
          new Year { Value = DateTime.Now.Date.Year - 4, Description = (DateTime.Now.Date.Year - 4).ToString() },
          new Year { Value = DateTime.Now.Date.Year - 5, Description = (DateTime.Now.Date.Year - 5).ToString() },
          new Year { Value = DateTime.Now.Date.Year - 6, Description = (DateTime.Now.Date.Year - 6).ToString() },
          new Year { Value = DateTime.Now.Date.Year - 7, Description = (DateTime.Now.Date.Year - 7).ToString() },
          new Year { Value = DateTime.Now.Date.Year - 8, Description = (DateTime.Now.Date.Year - 8).ToString() },
          new Year { Value = DateTime.Now.Date.Year - 9, Description = (DateTime.Now.Date.Year - 9).ToString() },
          new Year { Value = 0, Description = LanguageService.Default.GetString("SearchAll") }
        ];

        SelectedYear = Years.First(e => e.Value == DateTime.Now.Date.Year);
    }

    /// <summary>
    /// Gets or sets the clear command.
    /// </summary>
    /// <value>The clear command.</value>
    public RelayCommand? ClearCommand { get; private set; }

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
        ViewModelSelectionBlade<TestItem> selectionVM = new ViewModelSelectionBlade<TestItem>(_commonServices, Items, SelectedItems, ISynergy.Framework.Mvvm.Enumerations.SelectionModes.Single);
        selectionVM.Submitted += SelectionVM_Submitted;
        return CommonServices.NavigationService.OpenBladeAsync<ISelectionView>(this, selectionVM);
    }

    /// <summary>
    /// Selections the vm submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void SelectionVM_Submitted(object? sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionBlade<TestItem> vm)
            vm.Submitted -= SelectionVM_Submitted;

        SelectedItems = e.Result;
    }
}
