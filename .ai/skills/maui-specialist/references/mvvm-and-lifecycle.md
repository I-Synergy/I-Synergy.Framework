# MVVM Pattern & App Lifecycle
CommunityToolkit.Mvvm ViewModels, commands and bindings, plus MAUI app lifecycle events. Load this when building ViewModels or handling app lifecycle.

## MVVM Pattern (CommunityToolkit.Mvvm)

### ViewModel
```csharp
// File: ViewModels/BudgetListViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using {ApplicationName}.Mobile.Data;
using {ApplicationName}.Mobile.Models;

namespace {ApplicationName}.Mobile.ViewModels;

public partial class BudgetListViewModel : ObservableObject
{
    private readonly LocalDatabase _database;
    private readonly ISyncService _syncService;

    [ObservableProperty]
    private ObservableCollection<BudgetLocal> budgets = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public BudgetListViewModel(
        LocalDatabase database,
        ISyncService syncService)
    {
        _database = database;
        _syncService = syncService;
    }

    [RelayCommand]
    private async Task LoadBudgetsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var budgets = await _database.GetBudgetsAsync();
            Budgets = new ObservableCollection<BudgetLocal>(budgets);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load budgets";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;

        try
        {
            if (_syncService.IsOnline)
            {
                await _syncService.SyncDataAsync();
                await LoadBudgetsAsync();
            }
            else
            {
                ErrorMessage = "Cannot sync while offline";
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task DeleteBudgetAsync(BudgetLocal budget)
    {
        await _database.DeleteBudgetAsync(budget);

        if (_syncService.IsOnline)
        {
            // Queue for sync
            await _database.QueueOperationAsync("Delete", "Budget", budget.BudgetId);
        }

        await LoadBudgetsAsync();
    }
}
```

### Blazor Component Using ViewModel
```razor
@page "/budgets"
@inject BudgetListViewModel ViewModel
@implements IDisposable

<h1>Budgets</h1>

@if (ViewModel.IsLoading)
{
    <p>Loading budgets...</p>
}
else if (!string.IsNullOrEmpty(ViewModel.ErrorMessage))
{
    <div class="error">@ViewModel.ErrorMessage</div>
}
else
{
    <div class="budget-list">
        @foreach (var budget in ViewModel.Budgets)
        {
            <div class="budget-card">
                <h3>@budget.Name</h3>
                <p>@budget.Amount.ToString("C")</p>
                <button @onclick="() => DeleteBudget(budget)">Delete</button>
            </div>
        }
    </div>
}

<button @onclick="Refresh" disabled="@ViewModel.IsRefreshing">
    @(ViewModel.IsRefreshing ? "Syncing..." : "Sync")
</button>

@code {
    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadBudgetsCommand.ExecuteAsync(null);
    }

    private async Task DeleteBudget(BudgetLocal budget)
    {
        await ViewModel.DeleteBudgetCommand.ExecuteAsync(budget);
    }

    private async Task Refresh()
    {
        await ViewModel.RefreshCommand.ExecuteAsync(null);
    }

    public void Dispose()
    {
        // Clean up subscriptions
    }
}
```

## App Lifecycle

```csharp
// File: App.xaml.cs
public partial class App : Application
{
    private readonly LocalDatabase _database;
    private readonly ISyncService _syncService;

    public App(LocalDatabase database, ISyncService syncService)
    {
        InitializeComponent();

        _database = database;
        _syncService = syncService;

        MainPage = new MainPage();
    }

    protected override async void OnStart()
    {
        // App started
        await _database.InitializeAsync();

        if (_syncService.IsOnline)
        {
            _ = _syncService.SyncDataAsync();
        }
    }

    protected override void OnSleep()
    {
        // App backgrounded
    }

    protected override async void OnResume()
    {
        // App resumed
        if (_syncService.IsOnline)
        {
            await _syncService.SyncDataAsync();
        }
    }
}
```

