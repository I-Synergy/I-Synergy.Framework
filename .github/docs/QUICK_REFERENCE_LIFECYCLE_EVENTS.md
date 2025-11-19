# Quick Reference: Application Lifecycle Events

## One-Minute Overview

The `ApplicationLifecycleService` replaces message-based coordination with clean event-driven initialization management.

```csharp
// Three phases of application startup
???????????????????????????????
? 1. UiReady Event            ? ? Window ready for dialogs
? 2. ApplicationInitialized   ? ? Business logic ready
? 3. ApplicationLoaded Event  ? ? Full startup complete ?
???????????????????????????????
```

## For Application Developers

### Subscribe to Application Loaded Event

```csharp
public partial class App : Application
{
    private ApplicationLifecycleService? _lifecycleService;

    public App() : base()
    {
        _lifecycleService = _commonServices.ScopedContextService
        .GetRequiredService<ApplicationLifecycleService>();
        
        // ? Single point for post-startup operations
        _lifecycleService.ApplicationLoaded += async (s, e) => 
            await OnApplicationLoadedAsync();
    }

    private async Task OnApplicationLoadedAsync()
    {
     // Safe to navigate, show dialogs, etc.
        // Do auto-login here
    }
}
```

### Signal Initialization Complete

```csharp
protected override async Task InitializeApplicationAsync()
{
    // ... do initialization work ...
    await InitializeExceptionHandlingAsync();
    // ... load data, apply migrations, etc ...
    
    // ? Signal that app initialization is done
    _lifecycleService?.SignalApplicationInitialized();
}
```

## For Framework Developers

### Register in Dependency Injection

```csharp
// In MauiAppBuilderExtensions.cs
// ? Register as SINGLETON - lifecycle service lives for the entire application
appBuilder.Services.TryAddSingleton<ApplicationLifecycleService>();
```

### Signal UI Ready (from UI Component)

```csharp
// In EmptyView.cs
Loaded += (s, e) =>
{
    var lifecycleService = commonServices.ScopedContextService
      .GetRequiredService<ApplicationLifecycleService>();
    
    // ? Signal that UI is ready
    lifecycleService.SignalUiReady();
};
```

## Key Guarantees

| Guarantee | How |
|-----------|-----|
| **ApplicationLoaded fires exactly once** | Using `Interlocked` operations |
| **Works in any signal order** | Waits for both before firing |
| **Thread-safe** | Atomic compare-and-swap operations |
| **No race conditions** | Single coordination point |
| **No delays needed** | Event-driven (device-agnostic) |

## Lifecycle State Queries

```csharp
public bool IsUiReady { get; }              // Window created
public bool IsInitialized { get; }          // Business logic ready
public bool IsApplicationLoaded { get; }    // Both ready
```

## Common Patterns

### Auto-Login After App Loads

```csharp
private async Task OnApplicationLoadedAsync()
{
    var hasStoredToken = await _tokenService.HasValidTokenAsync();
    
    if (hasStoredToken)
  {
        await _authService.RefreshTokenAsync();
        await _navigationService.NavigateModalAsync<ShellViewModel>();
    }
    else
    {
  await _navigationService.NavigateModalAsync<SignInViewModel>();
    }
}
```

### Show Splash Screen During Init

```csharp
protected override async Task InitializeApplicationAsync()
{
    _commonServices.BusyService.StartBusy("Loading...");
    
    try
    {
        await InitializeExceptionHandlingAsync();
   // ... other init ...
    }
    finally
    {
        _lifecycleService?.SignalApplicationInitialized();
    }
}
```

### Listen to Individual Phases

```csharp
_lifecycleService.UiReady += (s, e) => 
{
    _logger.LogInformation("UI is ready");
};

_lifecycleService.ApplicationInitialized += (s, e) =>
{
    _logger.LogInformation("App initialization complete");
};

_lifecycleService.ApplicationLoaded += (s, e) =>
{
    _logger.LogInformation("App fully loaded - safe to navigate");
};
```

## What NOT to Do

? Don't use messages for lifecycle coordination
```csharp
// ? Wrong
MessengerService.Default.Send(new ApplicationLoadedMessage());
_commonServices.MessengerService.Register<ApplicationLoadedMessage>(...)
```

? Don't use flags for synchronization
```csharp
// ? Wrong
private bool _isHandlingInitialAuthentication = false;
```

? Don't use arbitrary delays
```csharp
// ? Wrong
await Task.Delay(500); // Magic number
```

? Don't have multiple navigation points
```csharp
// ? Wrong
if (someCondition) 
  await Navigate<ShellViewModel>();
else
    await Navigate<SignInViewModel>();
// Then later...
await Navigate<SignInViewModel>(); // Duplicate
```

## Migration Example

### Before (Message-Based)
```csharp
public App() : base()
{
  _commonServices.MessengerService.Register<ApplicationLoadedMessage>(
 this, async (m) => await ApplicationLoadedAsync(m));
}

private async Task ApplicationLoadedAsync(ApplicationLoadedMessage message)
{
 await _navigationService.NavigateModalAsync<SignInViewModel>();
}

protected override async Task InitializeApplicationAsync()
{
    MessengerService.Default.Send(new ApplicationInitializedMessage());
}
```

### After (Event-Based)
```csharp
public App() : base()
{
_lifecycleService = _commonServices.ScopedContextService
        .GetRequiredService<ApplicationLifecycleService>();
    
    _lifecycleService.ApplicationLoaded += async (s, e) => 
      await ApplicationLoadedAsync();
}

private async Task ApplicationLoadedAsync()
{
    await _navigationService.NavigateModalAsync<SignInViewModel>();
}

protected override async Task InitializeApplicationAsync()
{
    _lifecycleService?.SignalApplicationInitialized();
}
```

## Debugging Tips

### Enable Trace Logging
```csharp
// In appsettings.json
{
  "Logging": {
    "LogLevel": {
      "ISynergy.Framework.UI.Services.ApplicationLifecycleService": "Trace"
    }
  }
}
```

### Watch the Lifecycle
```csharp
_lifecycleService.UiReady += (s, e) => 
    Debug.WriteLine("? UiReady");

_lifecycleService.ApplicationInitialized += (s, e) =>
    Debug.WriteLine("? ApplicationInitialized");

_lifecycleService.ApplicationLoaded += (s, e) =>
    Debug.WriteLine("? ApplicationLoaded - Navigate now!");
```

### Check State at Any Time
```csharp
if (!_lifecycleService.IsUiReady)
    throw new InvalidOperationException("UI not ready yet!");

if (!_lifecycleService.IsInitialized)
    throw new InvalidOperationException("App not initialized yet!");

if (_lifecycleService.IsApplicationLoaded)
    Debug.WriteLine("Safe to navigate now");
```

## Related Documentation

- **MAUI_APPLICATION_LIFECYCLE_REFACTOR.md** - Full architecture documentation
- **BIOMETRIC_AUTH_PRACTICAL_EXAMPLE.md** - Real-world example with biometric auth
- **REFACTOR_IMPLEMENTATION_SUMMARY.md** - Implementation details and changes made

## Support

For questions or issues:
1. Check the full documentation
2. Review Sample.Maui implementation
3. Examine ApplicationLifecycleService source
4. Check structured logs at TRACE level
