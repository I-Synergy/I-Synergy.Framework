# MVVM Pattern & App Lifecycle
CommunityToolkit.Mvvm ViewModels, commands and bindings, plus MAUI app lifecycle events. Load this when building ViewModels or handling app lifecycle.

## MVVM Pattern (CommunityToolkit.Mvvm)

### ViewModel
```csharp
// File: ViewModels/{Entity}ListViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using {ApplicationName}.Mobile.Data;
using {ApplicationName}.Mobile.Models;

namespace {ApplicationName}.Mobile.ViewModels;

public partial class {Entity}ListViewModel : ObservableObject
{
    private readonly LocalDatabase _database;
    private readonly ISyncService _syncService;

    [ObservableProperty]
    private ObservableCollection<{Entity}Local> {entities} = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public {Entity}ListViewModel(
        LocalDatabase database,
        ISyncService syncService)
    {
        _database = database;
        _syncService = syncService;
    }

    [RelayCommand]
    private async Task Load{Entities}Async()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var {entities} = await _database.Get{Entities}Async();
            {Entities} = new ObservableCollection<{Entity}Local>({entities});
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load {entities}";
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
                await Load{Entities}Async();
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
    private async Task Delete{Entity}Async({Entity}Local {entity})
    {
        await _database.Delete{Entity}Async({entity});

        if (_syncService.IsOnline)
        {
            // Queue for sync
            await _database.QueueOperationAsync("Delete", "{Entity}", {entity}.{Entity}Id);
        }

        await Load{Entities}Async();
    }
}
```

### Blazor Component Using ViewModel
```razor
@page "/{entities}"
@inject {Entity}ListViewModel ViewModel
@implements IDisposable

<h1>{Entities}</h1>

@if (ViewModel.IsLoading)
{
    <p>Loading {entities}...</p>
}
else if (!string.IsNullOrEmpty(ViewModel.ErrorMessage))
{
    <div class="error">@ViewModel.ErrorMessage</div>
}
else
{
    <div class="{entity}-list">
        @foreach (var {entity} in ViewModel.{Entities})
        {
            <div class="{entity}-card">
                <h3>@({entity}.Name)</h3>
                <p>@({entity}.Amount.ToString("C"))</p>
                <button @onclick="() => Delete{Entity}({entity})">Delete</button>
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
        await ViewModel.Load{Entities}Command.ExecuteAsync(null);
    }

    private async Task Delete{Entity}({Entity}Local {entity})
    {
        await ViewModel.Delete{Entity}Command.ExecuteAsync({entity});
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

