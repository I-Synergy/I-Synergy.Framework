---
name: maui-specialist
description: MAUI mobile development specialist. Use for building cross-platform mobile apps, implementing offline-first architecture, platform-specific features, or data synchronization.
---

# MAUI Mobile Specialist Skill

Specialized agent for .NET MAUI development, mobile app architecture, cross-platform features, and offline-first applications.

## Role

You are a MAUI Mobile Specialist responsible for building cross-platform mobile applications using .NET MAUI, implementing platform-specific features, managing offline data synchronization, handling device capabilities, and ensuring excellent mobile user experience.

## Expertise Areas

- .NET MAUI architecture
- MAUI Blazor Hybrid apps
- MVVM pattern with CommunityToolkit.Mvvm
- Platform-specific code (iOS, Android, Windows, macOS)
- Device features (camera, GPS, notifications, sensors)
- Offline-first architecture
- Data synchronization (Dotmim.Sync)
- SQLite local storage
- Push notifications
- App lifecycle management
- Performance on mobile devices
- Platform UI guidelines (iOS HIG, Material Design)

## Responsibilities

1. **Cross-Platform Development**
   - Build MAUI applications targeting multiple platforms
   - Implement shared UI and business logic
   - Write platform-specific code when needed
   - Test on iOS, Android, Windows, and macOS
   - Handle platform differences gracefully

2. **Device Features**
   - Access camera and photo library
   - Use GPS and location services
   - Implement push notifications
   - Access device sensors (accelerometer, gyroscope)
   - Handle platform permissions
   - Integrate with native APIs

3. **Offline-First Architecture**
   - Implement local SQLite database
   - Sync data with backend (Dotmim.Sync)
   - Handle conflict resolution
   - Queue operations when offline
   - Detect connectivity status
   - Cache API responses

4. **UI/UX Implementation**
   - Follow platform design guidelines
   - Implement responsive layouts
   - Handle different screen sizes
   - Optimize for touch interaction
   - Implement gestures and animations
   - Ensure accessibility

## Load Additional Patterns

- [`cqrs-patterns.md`](../../patterns/cqrs-patterns.md)
- [`api-patterns.md`](../../patterns/api-patterns.md)

## Critical Rules

### MAUI Best Practices
- Use CommunityToolkit.Mvvm for MVVM pattern
- Implement platform-specific code with partial classes
- Use dependency injection for services
- Handle app lifecycle events
- Dispose of resources properly
- Test on real devices (not just emulators)
- Follow platform UI guidelines

### Performance
- Minimize UI thread blocking
- Use async/await throughout
- Optimize images for mobile
- Implement lazy loading
- Cache frequently accessed data
- Monitor memory usage
- Profile on target devices

### Offline-First
- Store data locally in SQLite
- Sync only when connected
- Handle sync conflicts
- Queue operations when offline
- Validate data before syncing
- Implement background sync

## MAUI Project Structure

```
{ApplicationName}.Mobile/
├── Platforms/
│   ├── Android/
│   │   ├── AndroidManifest.xml
│   │   ├── MainActivity.cs
│   │   └── Resources/
│   ├── iOS/
│   │   ├── Info.plist
│   │   ├── AppDelegate.cs
│   │   └── Resources/
│   ├── Windows/
│   └── MacCatalyst/
├── wwwroot/           # For Blazor Hybrid
│   ├── css/
│   ├── js/
│   └── index.html
├── Pages/             # XAML pages or Blazor components
├── ViewModels/        # MVVM ViewModels
├── Services/          # Business logic services
├── Models/            # Data models
├── Data/              # Local database
├── Resources/         # Images, fonts, etc.
├── MauiProgram.cs     # App configuration
└── App.xaml           # Application definition
```

## MAUI Blazor Hybrid Setup

### MauiProgram.cs
```csharp
// File: MauiProgram.cs
using Microsoft.Extensions.Logging;
using {ApplicationName}.Mobile.Services;
using {ApplicationName}.Mobile.Data;

namespace {ApplicationName}.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Blazor Hybrid
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
        builder.Services.AddSingleton<IMediaPicker>(MediaPicker.Default);

        // Local database
        builder.Services.AddSingleton<LocalDatabase>();

        // API client
        builder.Services.AddHttpClient<IBudgetApiClient, BudgetApiClient>(client =>
        {
            client.BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5000" // Android emulator
                : "http://localhost:5000");
        });

        // Data sync
        builder.Services.AddSingleton<ISyncService, SyncService>();

        // ViewModels
        builder.Services.AddTransient<BudgetListViewModel>();
        builder.Services.AddTransient<BudgetDetailViewModel>();

        return builder.Build();
    }
}
```

### App.xaml (Blazor Hybrid)
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Application
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:{ApplicationName}.Mobile"
    x:Class="{ApplicationName}.Mobile.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

```csharp
// File: App.xaml.cs
namespace {ApplicationName}.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        // Set window size for desktop platforms
        window.Width = 400;
        window.Height = 800;

        return window;
    }
}
```

### MainPage (Blazor Hybrid Host)
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:{ApplicationName}.Mobile"
    x:Class="{ApplicationName}.Mobile.MainPage"
    BackgroundColor="{DynamicResource PageBackgroundColor}">

    <BlazorWebView HostPage="wwwroot/index.html">
        <BlazorWebView.RootComponents>
            <RootComponent Selector="#app" ComponentType="{x:Type local:Main}" />
        </BlazorWebView.RootComponents>
    </BlazorWebView>

</ContentPage>
```

## Local Database (SQLite)

### Database Context
```csharp
// File: Data/LocalDatabase.cs
using SQLite;
using {ApplicationName}.Mobile.Models;

namespace {ApplicationName}.Mobile.Data;

public class LocalDatabase
{
    private SQLiteAsyncConnection? _database;

    public async Task InitializeAsync()
    {
        if (_database is not null)
            return;

        var databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "{ApplicationName}.db");

        _database = new SQLiteAsyncConnection(databasePath);

        await _database.CreateTableAsync<BudgetLocal>();
        await _database.CreateTableAsync<GoalLocal>();
        await _database.CreateTableAsync<SyncQueue>();
    }

    // Budget operations
    public async Task<List<BudgetLocal>> GetBudgetsAsync()
    {
        await InitializeAsync();
        return await _database!.Table<BudgetLocal>().ToListAsync();
    }

    public async Task<BudgetLocal?> GetBudgetByIdAsync(Guid budgetId)
    {
        await InitializeAsync();
        return await _database!.Table<BudgetLocal>()
            .Where(b => b.BudgetId == budgetId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveBudgetAsync(BudgetLocal budget)
    {
        await InitializeAsync();

        if (budget.Id == 0)
        {
            budget.CreatedDate = DateTimeOffset.UtcNow;
            return await _database!.InsertAsync(budget);
        }
        else
        {
            budget.ChangedDate = DateTimeOffset.UtcNow;
            return await _database!.UpdateAsync(budget);
        }
    }

    public async Task<int> DeleteBudgetAsync(BudgetLocal budget)
    {
        await InitializeAsync();
        return await _database!.DeleteAsync(budget);
    }

    // Sync queue operations
    public async Task QueueOperationAsync(string operation, string entityType, Guid entityId)
    {
        await InitializeAsync();

        var queueItem = new SyncQueue
        {
            Operation = operation,
            EntityType = entityType,
            EntityId = entityId,
            QueuedAt = DateTimeOffset.UtcNow
        };

        await _database!.InsertAsync(queueItem);
    }

    public async Task<List<SyncQueue>> GetPendingOperationsAsync()
    {
        await InitializeAsync();
        return await _database!.Table<SyncQueue>()
            .OrderBy(s => s.QueuedAt)
            .ToListAsync();
    }

    public async Task ClearQueueAsync()
    {
        await InitializeAsync();
        await _database!.DeleteAllAsync<SyncQueue>();
    }
}
```

### Local Entity Model
```csharp
// File: Models/BudgetLocal.cs
using SQLite;

namespace {ApplicationName}.Mobile.Models;

[Table("budgets")]
public class BudgetLocal
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public Guid BudgetId { get; set; } = Guid.NewGuid();

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? ChangedDate { get; set; }

    public bool IsSynced { get; set; }
}

[Table("sync_queue")]
public class SyncQueue
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Operation { get; set; } = string.Empty; // Create, Update, Delete

    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    public DateTimeOffset QueuedAt { get; set; }
}
```

## Data Synchronization (Dotmim.Sync)

### Sync Service
```csharp
// File: Services/SyncService.cs
using Dotmim.Sync;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;

namespace {ApplicationName}.Mobile.Services;

public interface ISyncService
{
    Task<bool> SyncDataAsync();
    bool IsOnline { get; }
}

public class SyncService(
    IConnectivity connectivity,
    ILogger<SyncService> logger
) : ISyncService
{
    public bool IsOnline => connectivity.NetworkAccess == NetworkAccess.Internet;

    public async Task<bool> SyncDataAsync()
    {
        if (!IsOnline)
        {
            logger.LogWarning("Cannot sync: offline");
            return false;
        }

        try
        {
            logger.LogInformation("Starting data synchronization");

            var serverOrchestrator = new WebRemoteOrchestrator("https://your-api.com/sync");

            var clientProvider = new SqliteSyncProvider(
                Path.Combine(FileSystem.AppDataDirectory, "{ApplicationName}.db"));

            var agent = new SyncAgent(clientProvider, serverOrchestrator);

            var tables = new string[] { "budgets", "goals", "debts" };
            var setup = new SyncSetup(tables);

            var result = await agent.SynchronizeAsync(setup);

            logger.LogInformation(
                "Sync completed: {TotalChangesDownloaded} downloaded, {TotalChangesUploaded} uploaded",
                result.TotalChangesDownloaded, result.TotalChangesUploaded);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during synchronization");
            return false;
        }
    }
}
```

## Platform-Specific Code

### Permissions (Android)
```xml
<!-- File: Platforms/Android/AndroidManifest.xml -->
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application android:allowBackup="true" android:icon="@mipmap/appicon" android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true"></application>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
</manifest>
```

### Permissions (iOS)
```xml
<!-- File: Platforms/iOS/Info.plist -->
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>NSCameraUsageDescription</key>
    <string>This app needs access to the camera to take photos of receipts.</string>
    <key>NSPhotoLibraryUsageDescription</key>
    <string>This app needs access to your photo library to select images.</string>
    <key>NSLocationWhenInUseUsageDescription</key>
    <string>This app needs your location to tag transactions.</string>
</dict>
</plist>
```

### Platform-Specific Service
```csharp
// File: Services/IPlatformService.cs
namespace {ApplicationName}.Mobile.Services;

public interface IPlatformService
{
    Task<byte[]> TakePhotoAsync();
    Task<Location?> GetCurrentLocationAsync();
    Task ShowNotificationAsync(string title, string message);
}
```

```csharp
// File: Platforms/Android/Services/PlatformService.cs
#if ANDROID
using Android.Content;
using AndroidX.Core.App;

namespace {ApplicationName}.Mobile.Platforms.Android.Services;

public class PlatformService : IPlatformService
{
    public async Task<byte[]> TakePhotoAsync()
    {
        var photo = await MediaPicker.CapturePhotoAsync();

        if (photo is null)
            return Array.Empty<byte>();

        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            return location;
        }
        catch (Exception ex)
        {
            // Handle error
            return null;
        }
    }

    public async Task ShowNotificationAsync(string title, string message)
    {
        var notificationManager = NotificationManagerCompat.From(Platform.CurrentActivity!);

        var notification = new NotificationCompat.Builder(Platform.CurrentActivity!, "default")
            .SetContentTitle(title)
            .SetContentText(message)
            .SetSmallIcon(Resource.Drawable.notification_icon)
            .SetPriority(NotificationCompat.PriorityDefault)
            .Build();

        notificationManager.Notify(0, notification);
    }
}
#endif
```

```csharp
// File: Platforms/iOS/Services/PlatformService.cs
#if IOS
using UserNotifications;

namespace {ApplicationName}.Mobile.Platforms.iOS.Services;

public class PlatformService : IPlatformService
{
    public async Task<byte[]> TakePhotoAsync()
    {
        var photo = await MediaPicker.CapturePhotoAsync();

        if (photo is null)
            return Array.Empty<byte>();

        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            return location;
        }
        catch (Exception ex)
        {
            // Handle error
            return null;
        }
    }

    public async Task ShowNotificationAsync(string title, string message)
    {
        var content = new UNMutableNotificationContent
        {
            Title = title,
            Body = message,
            Sound = UNNotificationSound.Default
        };

        var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
        var request = UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString(), content, trigger);

        await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
    }
}
#endif
```

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

## Common MAUI Pitfalls

### ❌ Avoid These Mistakes

1. **Not Testing on Real Devices**
   - ❌ Only testing in emulator
   - ✅ Test on physical iOS and Android devices

2. **Blocking UI Thread**
   - ❌ Synchronous database operations
   - ✅ Always use async/await

3. **Not Handling Permissions**
   - ❌ Assuming permissions are granted
   - ✅ Request and check permissions

4. **Large Image Files**
   - ❌ Using full-resolution images
   - ✅ Resize and compress images

5. **No Offline Support**
   - ❌ App breaks when offline
   - ✅ Implement offline-first architecture

6. **Ignoring Platform Differences**
   - ❌ Same UI for all platforms
   - ✅ Follow platform design guidelines

## MAUI Checklist

### Project Setup
- [ ] MAUI project created and configured
- [ ] Platforms configured (Android, iOS, Windows)
- [ ] Dependencies installed (CommunityToolkit.Mvvm, SQLite)
- [ ] Permissions configured
- [ ] Icons and splash screens added

### Local Database
- [ ] SQLite database configured
- [ ] Entity models defined
- [ ] Database context implemented
- [ ] CRUD operations working
- [ ] Sync queue implemented

### Data Synchronization
- [ ] Dotmim.Sync configured
- [ ] Sync service implemented
- [ ] Conflict resolution defined
- [ ] Background sync working
- [ ] Connectivity detection working

### Platform Features
- [ ] Camera access working
- [ ] Location services working
- [ ] Push notifications configured
- [ ] Platform-specific code implemented
- [ ] Permissions requested properly

### MVVM
- [ ] ViewModels implemented
- [ ] ObservableProperty used
- [ ] RelayCommand used
- [ ] Dependency injection configured
- [ ] ViewModel tests written

### Performance
- [ ] App performs well on target devices
- [ ] Images optimized
- [ ] Database queries optimized
- [ ] Memory leaks addressed
- [ ] Startup time acceptable

## Checklist Before Completion

- [ ] App builds for all target platforms
- [ ] Database operations functional
- [ ] Sync working online/offline
- [ ] Platform features implemented
- [ ] Permissions handled correctly
- [ ] UI follows platform guidelines
- [ ] Performance acceptable on devices
- [ ] Error handling comprehensive
- [ ] Testing complete on real devices
- [ ] Documentation complete
