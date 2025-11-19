# MAUI Application Lifecycle Refactor: Event-Driven Architecture

## Overview

This refactor replaces the message-based application initialization coordination with a clean, event-driven architecture using the new `ApplicationLifecycleService`. This eliminates race conditions, reduces workarounds, and provides a clear lifecycle state machine for MAUI applications.

## Problems Solved

### 1. **Race Conditions During Startup**
**Before**: Multiple overlapping biometric prompts and duplicate navigation attempts
**After**: Clear sequential lifecycle with guaranteed ordering through atomic operations

### 2. **Message-Based Coordination (Anti-pattern)**
**Before**: 
```csharp
_commonServices.MessengerService.Send(new ApplicationUiReadyMessage());
_commonServices.MessengerService.Send(new ApplicationInitializedMessage());
_commonServices.MessengerService.Send(new ApplicationLoadedMessage());
```

**After**: Proper event-based coordination
```csharp
_lifecycleService.SignalUiReady();
_lifecycleService.SignalApplicationInitialized();
// ApplicationLoaded event fires automatically when both signals received
_lifecycleService.ApplicationLoaded += OnApplicationLoaded;
```

### 3. **Arbitrary Delays and Heuristics**
**Before**: 
```csharp
await Task.Delay(500); // Magic number - unreliable
```

**After**: Event-driven coordination - no delays needed

### 4. **Flag-Based Synchronization**
**Before**: 
```csharp
private volatile bool _uiReady;
private volatile bool _initialized;
private int _loadedPublished;
```

**After**: Encapsulated in ApplicationLifecycleService with atomic operations

## Architecture

### ApplicationLifecycleService

```csharp
public sealed class ApplicationLifecycleService : IDisposable
{
    // Events represent distinct lifecycle phases
    public event EventHandler<EventArgs>? UiReady;
    public event EventHandler<EventArgs>? ApplicationInitialized;
    public event EventHandler<EventArgs>? ApplicationLoaded;

    // Signal methods called from framework components
public void SignalUiReady();
    public void SignalApplicationInitialized();

    // Status query methods
 public bool IsUiReady { get; }
    public bool IsInitialized { get; }
    public bool IsApplicationLoaded { get; }
}
```

### Lifecycle Flow

```
???????????????????????????????????????????????????????????????????
?   MAUI Application Startup        ?
???????????????????????????????????????????????????????????????????
      ?
       ??? [1] App Constructor
           ?       - Services initialized
      ?       - EmptyView set as MainPage
    ?       - InitializeApplication() started
    ?
??????????????????????
       ?         ?
         [2] EmptyView.Loaded      [3] InitializeApplicationAsync
              Event        Background Task
   ?       ?
           ?        ?
   Signal UiReady() Signal ApplicationInitialized()
            ?         ?
       ??????????????????????
        ?
    ????????????????????
 ?         ?
              Both signals         Check: Both ready?
  received?       ?
         ?        ? YES
          ?   ????????
  ?            ?
   ????????????? Raise ApplicationLoaded
          ?  Event (Once, atomically)
  ?
     ?????????????????????
    ?  ?
           Navigate based on   Navigate based on
           authentication             authentication
            ?    ?
           ?????????????????????
      ?
      App fully operational
```

## Implementation Details

### 1. EmptyView - UI Readiness Signal

```csharp
Loaded += (s, e) =>
{
    try
    {
        var lifecycleService = commonServices.ScopedContextService
  .GetRequiredService<ApplicationLifecycleService>();
        lifecycleService.SignalUiReady();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error signaling UI ready: {ex}");
    }
};
```

**When**: Raised when the window is created and EmptyView is displayed
**What**: First safe point for dialogs and modal navigation
**Why**: Before this, MAUI's window isn't fully ready for UI operations

### 2. Application.InitializeApplicationAsync - Initialization Signal

```csharp
protected override async Task InitializeApplicationAsync()
{
    // ... perform initialization, load data, apply migrations ...
    
    // Signal completion
    _lifecycleService?.SignalApplicationInitialized();
}
```

**When**: Called after all backend initialization is complete
**What**: Business logic, database setup, etc. are ready
**Why**: Separates UI readiness from business logic readiness

### 3. Application.OnApplicationLoaded - Coordination Point

```csharp
protected virtual void OnApplicationLoaded(object? sender, EventArgs e)
{
    _logger?.LogTrace("Application fully loaded");
}
```

**When**: Raised automatically when BOTH signals received (atomically)
**What**: Application is fully operational
**Why**: Single, guaranteed point for post-load operations

## Migration Guide for Applications

### Old Pattern (Message-Based)

```csharp
public partial class App : Application
{
    public App() : base()
    {
     _commonServices.MessengerService.Register<ApplicationLoadedMessage>(
            this, 
          async (m) => await ApplicationLoadedAsync(m));
    }

    protected override async Task InitializeApplicationAsync()
    {
        // ... init ...
        MessengerService.Default.Send(new ApplicationInitializedMessage());
    }

    private async Task ApplicationLoadedAsync(ApplicationLoadedMessage message)
    {
        // Navigate based on auth state
     await _navigationService.NavigateModalAsync<SignInViewModel>();
    }

    protected override void Dispose(bool disposing)
    {
        MessengerService.Default.Unregister<ApplicationLoadedMessage>(this);
    }
}
```

### New Pattern (Event-Based)

```csharp
public partial class App : Application
{
    private ApplicationLifecycleService? _lifecycleService;

    public App() : base()
    {
        _lifecycleService = _commonServices.ScopedContextService
            .GetRequiredService<ApplicationLifecycleService>();
  
        // Clean event subscription
        _lifecycleService.ApplicationLoaded += async (s, e) => await ApplicationLoadedAsync();
    }

    protected override async Task InitializeApplicationAsync()
    {
     // ... init ...
      // Simply signal completion - framework handles the rest
        _lifecycleService?.SignalApplicationInitialized();
    }

 private async Task ApplicationLoadedAsync()
    {
        // Navigate based on auth state
        await _navigationService.NavigateModalAsync<SignInViewModel>();
    }

    protected override void Dispose(bool disposing)
    {
   // Framework handles cleanup automatically
        base.Dispose(disposing);
    }
}
```

## Key Benefits

### 1. **No Race Conditions**
- Atomic operations guarantee ApplicationLoaded fires exactly once
- Clear ordering of lifecycle phases prevents overlapping operations
- No more arbitrary delays needed

### 2. **Cleaner Code**
- No more message registrations/unregistrations
- No flag-based synchronization logic
- Clear intent through events vs. messages

### 3. **Better Separation of Concerns**
- UI readiness tracked independently from business logic readiness
- Each phase has clear responsibility
- Easy to add new lifecycle phases if needed

### 4. **Improved Debuggability**
- Structured logging at each phase
- Clear state queries (IsUiReady, IsInitialized, IsApplicationLoaded)
- Event-based debugging is more straightforward

### 5. **Resilient Design**
- Service validates all inputs
- Comprehensive exception handling in event invocations
- Doesn't propagate errors from event handlers

## Thread Safety

The `ApplicationLifecycleService` uses `Interlocked` operations for thread-safe state management:

```csharp
// Atomic compare-and-swap ensures only first signal is processed
if (Interlocked.CompareExchange(ref _uiReadyPublished, 1, 0) == 0)
{
    // This block executes exactly once, no matter how many threads call SignalUiReady
    UiReady?.Invoke(this, EventArgs.Empty);
}
```

This prevents:
- Multiple event invocations
- Race conditions between signals
- Lost or duplicate navigation attempts

## Backward Compatibility

The old message-based approach is **NOT** maintained for cleanliness. Applications using the framework should migrate to:

1. **No longer use** `ApplicationLoadedMessage`, `ApplicationUiReadyMessage`, `ApplicationInitializedMessage`
2. **Instead use** `ApplicationLifecycleService` events
3. **Remove** all flag-based coordination from app classes
4. **Remove** arbitrary delays (`Task.Delay()`) from initialization

## Logging Strategy

The service provides structured logging at key points:

```
[TRACE] ApplicationLifecycleService instantiated
[TRACE] SignalUiReady: UI framework is now ready
[TRACE] SignalApplicationInitialized: Application initialization is complete
[TRACE] ApplicationLoaded: Both UI and initialization complete, raising ApplicationLoaded
[WARN]  SignalUiReady called multiple times; ignoring subsequent calls
```

Applications should listen for:
- ERROR level: Exceptions in event handlers (indicates bugs in handlers)
- TRACE level: Lifecycle progression

## Example Implementation

See `samples/Sample.Maui/App.xaml.cs` for a complete, working example.

Key highlights:
```csharp
// In constructor
_lifecycleService = _commonServices.ScopedContextService
    .GetRequiredService<ApplicationLifecycleService>();
_lifecycleService.ApplicationLoaded += async (s, e) => await ApplicationLoadedAsync();

// In InitializeApplicationAsync
protected override async Task InitializeApplicationAsync()
{
    // Do all your initialization here
    await InitializeExceptionHandlingAsync();
    // ... other init code ...
    
    // Signal completion
    _lifecycleService?.SignalApplicationInitialized();
}

// Handle the loaded event
private async Task ApplicationLoadedAsync()
{
    // This runs when UI is ready AND init is complete
    // Safe to navigate, show dialogs, etc.
 if (navigateToAuthentication)
    {
        await _navigationService.NavigateModalAsync<SignInViewModel>();
    }
}
```

## Testing Considerations

The lifecycle service is designed for testing:

```csharp
[Fact]
public void ApplicationLoaded_FiresOnceWhenBothSignalsReceived()
{
    var service = new ApplicationLifecycleService(_logger);
    var loaded = false;
    
    service.ApplicationLoaded += (s, e) => { loaded = true; };
    
    service.SignalUiReady();
    Assert.False(loaded); // Not yet
    
    service.SignalApplicationInitialized();
    Assert.True(loaded); // Now fired
}

[Fact]
public void Signals_CanArriveInAnyOrder()
{
    var service = new ApplicationLifecycleService(_logger);
    var loaded = false;
    
    service.ApplicationLoaded += (s, e) => { loaded = true; };
    
    // Try reverse order
    service.SignalApplicationInitialized();
    Assert.False(loaded);
    
    service.SignalUiReady();
    Assert.True(loaded); // Still works!
}
```

## Summary

This refactor provides a solid foundation for MAUI applications by:

1. ? Eliminating race conditions through atomic operations
2. ? Replacing fragile message-based coordination with clean events
3. ? Removing arbitrary delays and heuristics
4. ? Providing clear lifecycle phases
5. ? Maintaining thread safety
6. ? Improving code clarity and maintainability

Applications can now rely on a deterministic startup sequence with clear separation between UI readiness and business logic readiness.
