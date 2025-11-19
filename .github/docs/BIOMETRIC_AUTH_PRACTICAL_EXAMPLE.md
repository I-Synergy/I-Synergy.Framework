# Practical Example: Biometric Authentication Initialization

## The Problem (Before Refactor)

In the old architecture, this code in `Sample.Maui/App.xaml.cs` had serious issues:

```csharp
private bool _isHandlingInitialAuthentication = false; // ? Flag-based sync

private async void AuthenticationStateChangedAsync(Profile? profile)
{
    try
    {
      // ? WORKAROUND: Skip navigation during initial authentication
        if (_isHandlingInitialAuthentication)
        {
            _logger.LogInformation("Skipping navigation during initial authentication");
            return; // Hide the problem instead of solving it
  }
        
        // ... handle auth state change ...
    }
}

private async Task ApplicationLoadedAsync(ApplicationLoadedMessage message)
{
  try
    {
        var hasRefreshToken = await _authenticationService.HasStoredRefreshTokenAsync();

        if (hasRefreshToken)
{
            // ? WORKAROUND: Set flag to prevent duplicate navigation
    _isHandlingInitialAuthentication = true;

    // ? MAGIC DELAY: Hoping 500ms is enough
            MainThread.BeginInvokeOnMainThread(async () =>
            {
      await Task.Delay(500); // Not reliable on slow devices
   
     try
     {
      var refreshSuccess = await _authenticationService.TryRefreshTokenAsync();
  
            // ? WORKAROUND: Reset flag after attempt
_isHandlingInitialAuthentication = false;

       if (refreshSuccess)
       {
       var context = _commonServices.ScopedContextService.GetRequiredService<IContext>();
       if (context?.Profile is not null)
        {
    await _navigationService.NavigateModalAsync<ShellViewModel>();
        return;
                 }
  }

       await _navigationService.NavigateModalAsync<SignInViewModel>();
     }
            catch (Exception ex)
    {
      _isHandlingInitialAuthentication = false;
     await _navigationService.NavigateModalAsync<SignInViewModel>();
        }
         });
        }
     else
    {
            await _navigationService.NavigateModalAsync<SignInViewModel>();
        }

    // ? DUPLICATE NAVIGATION: This executes immediately, before deferred callback
      await _navigationService.NavigateModalAsync<SignInViewModel>();
    }
    finally
    {
        _commonServices.BusyService.StopBusy();
    }
}
```

### Issues with this code:

1. **Race Condition**: Navigation from both `AuthenticationStateChangedAsync` and `ApplicationLoadedAsync`
2. **Flag Abuse**: `_isHandlingInitialAuthentication` is a code smell indicating wrong design
3. **Unreliable Timing**: `Task.Delay(500)` fails on slow devices or emulators
4. **Duplicate Navigation**: Last line always navigates regardless of path taken
5. **Complex State**: Hard to reason about when each code path executes
6. **Biometric Issues**: Prompts may appear before window is ready

## The Solution (After Refactor)

### Architecture Guarantee

The `ApplicationLifecycleService` provides an ironclad guarantee:
- **EmptyView.Loaded** event fires ? UI is ready
- **InitializeApplicationAsync** completes ? Business logic is ready
- **ApplicationLoaded event** fires ? Both are ready, safe to navigate

```
EmptyView.Loaded
    ? (signals UI ready)
    ??? (window ready for dialogs)
    ?
InitializeApplicationAsync
    ? (when done, signals app initialized)
    ??? (data loaded, migrations applied)
    ?
    ? (both signals received)
    ?
ApplicationLoaded event ? Safe navigation point
```

### New Implementation

```csharp
public partial class App : Application
{
    private ApplicationLifecycleService? _lifecycleService;

    public App() : base()
    {
        try
        {
    InitializeComponent();

    // Get the lifecycle service for event-driven coordination
            _lifecycleService = _commonServices.ScopedContextService
           .GetRequiredService<ApplicationLifecycleService>();

            // Subscribe to the ApplicationLoaded event
            // ? Clean, single point for post-load operations
      _lifecycleService.ApplicationLoaded += async (s, e) => await ApplicationLoadedAsync();

        // ... other message handlers ...
     }
        catch (Exception ex)
     {
            _logger?.LogCritical(ex, "Error in App constructor");
        }
    }

    protected override async Task InitializeApplicationAsync()
    {
        try
        {
          _logger?.LogTrace("Starting application initialization");

            // Initialize exception handling first
 await InitializeExceptionHandlingAsync();

  // Perform backend initialization
            if (_commonServices?.BusyService != null)
 {
             _commonServices.BusyService.UpdateMessage("Initializing...");
          await Task.Delay(2000); // Simulated work
          _commonServices.BusyService.UpdateMessage("Loading data...");
       await Task.Delay(2000);
      }
        }
        catch (Exception ex)
        {
        _logger?.LogError(ex, "Error during initialization");
         throw;
        }

 // ? Signal completion - framework handles the rest
        _logger?.LogTrace("Application initialization complete, signaling framework");
        _lifecycleService?.SignalApplicationInitialized();
    }

    /// <summary>
    /// Called when both UI is ready AND application initialization is complete.
    /// This is the ONLY safe place to navigate or show dialogs at startup.
    /// ? No flags needed
    /// ? No duplicate navigation
    /// ? No arbitrary delays
    /// ? Clear intent
    /// </summary>
    private async Task ApplicationLoadedAsync()
    {
    try
    {
     _commonServices.BusyService.StartBusy();

            _logger.LogInformation("Application loaded: checking for auto-login");

            // Check for stored refresh token
   var hasRefreshToken = await _authenticationService.HasStoredRefreshTokenAsync();

          if (hasRefreshToken)
            {
            try
{
   // Try to refresh the token
  var refreshSuccess = await _authenticationService.TryRefreshTokenAsync();
            
   if (refreshSuccess)
         {
           var context = _commonServices.ScopedContextService
       .GetRequiredService<IContext>();
          
  if (context?.Profile is not null)
    {
   // ? Safe to navigate - UI is ready
             await _navigationService.NavigateModalAsync<ShellViewModel>();
       return;
       }
}
   }
        catch (Exception ex)
      {
             _logger?.LogError(ex, "Error during token refresh");
        }
       }

  // No valid token or refresh failed - go to sign in
      // ? Single, clear navigation point
   await _navigationService.NavigateModalAsync<SignInViewModel>();
        }
     catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in ApplicationLoaded");
  }
 finally
        {
 _commonServices.BusyService.StopBusy();
        }
    }

    // ... rest of class unchanged ...
}
```

## Comparing the Two Approaches

| Aspect | Before | After |
|--------|--------|-------|
| **Race Conditions** | Multiple overlapping paths ? | Single coordinated point ? |
| **Navigation Sources** | 3+ locations | 1 location |
| **Synchronization** | Flags, delays, heuristics ? | Atomic operations ? |
| **Reliability** | Fails on slow devices ? | Works reliably ? |
| **Code Clarity** | Complex state logic ? | Linear flow ? |
| **Duplicate Code** | Yes (navigation appears 3 times) ? | No ? |
| **Testability** | Hard to test workarounds ? | Easy to test ? |
| **Maintainability** | Fragile to changes ? | Robust ? |

## How the New Code Prevents Issues

### Problem: Biometric Prompt Before Window Ready

**Before**: Prompt triggered during `InitializeApplicationAsync` when window not ready
```
InitializeApplicationAsync runs (window not ready)
  ? TryRefreshTokenAsync called
    ? Biometric service shows prompt
      ? Prompt fails (no window) or appears multiple times ?
```

**After**: Biometric prompt only triggered in `ApplicationLoadedAsync` when window ready
```
EmptyView.Loaded
  ? Window is now ready
InitializeApplicationAsync (background)
  ? No UI operations
ApplicationLoaded event
  ? ApplicationLoadedAsync runs
    ? TryRefreshTokenAsync called
      ? Biometric prompt shows correctly ?
```

### Problem: Multiple Navigation Attempts

**Before**: Code paths converge causing duplicate navigation
```
Event from AuthenticationStateChanged
  ?? Navigate to Shell ? First attempt
Deferred callback from MainThread.BeginInvokeOnMainThread
  ?? Navigate to Shell ? Second attempt
Direct navigation at end of method
  ?? Navigate to SignIn ? Third attempt
```

**After**: Single navigation point ensures one attempt
```
ApplicationLoaded event (fires once)
  ?? ApplicationLoadedAsync
      ?? One navigation based on auth state ?
```

### Problem: Race on Slow Devices

**Before**: Arbitrary delay not suitable for all devices
```
await Task.Delay(500); // Might not be enough on slow device
```

**After**: Event-based coordination is device-agnostic
```
// Framework waits for both signals regardless of device speed
UiReady signal + Initialized signal ? ApplicationLoaded fires
```

## Biometric Service Integration Example

Here's how a biometric service might integrate now:

```csharp
public interface IBiometricService
{
    Task<bool> IsAvailableAsync();
    Task<BioAuthResult> AuthenticateAsync(string reason);
}

public class BioAuthResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

// In AuthenticationService
public async Task<bool> TryRefreshTokenAsync(CancellationToken cancellationToken = default)
{
    if (!_tokenStorageService.HasRefreshToken())
        return false;

    // ? Now called only after UI is ready (from ApplicationLoadedAsync)
  if (await _biometricService.IsAvailableAsync())
    {
        var bioResult = await _biometricService.AuthenticateAsync("Authenticate to continue");
        
      if (!bioResult.Success)
        {
         _logger.LogInformation("Biometric authentication cancelled");
        return false;
    }
    }

    // Proceed with token refresh...
    var token = _tokenStorageService.GetRefreshToken();
    var newAccessToken = await _apiClient.RefreshTokenAsync(token);
    
    if (newAccessToken != null)
    {
        _tokenStorageService.SaveAccessToken(newAccessToken);
        SetProfile(newAccessToken);
        return true;
    }

    return false;
}
```

**Why this works now:**
1. `TryRefreshTokenAsync` is only called from `ApplicationLoadedAsync`
2. `ApplicationLoadedAsync` only fires when window is ready
3. Biometric service prompts appear reliably
4. No overlapping prompts
5. Clear, linear flow

## Testing the New Architecture

```csharp
[TestClass]
public class AppLifecycleTests
{
    [TestMethod]
    public async Task ApplicationLoadedAsync_OnlyCalledOnce()
    {
        // Arrange
        var app = new App();
        int navigationCount = 0;
 
        app._navigationService = Mock.Of<INavigationService>(
            ns => ns.NavigateModalAsync(It.IsAny<Type>()) 
        == Task.CompletedTask
 );

        // Act
        var lifecycleService = app._lifecycleService;
  lifecycleService.SignalUiReady();
      lifecycleService.SignalApplicationInitialized();
 
 // Wait for ApplicationLoaded event
        await Task.Delay(100);

        // Assert
        app._navigationService.Verify(
       ns => ns.NavigateModalAsync(It.IsAny<Type>()), 
          Times.Once()); // ? Navigated exactly once
    }

    [TestMethod]
    public async Task ApplicationLoaded_WorksRegardlessOfSignalOrder()
    {
    // Arrange
        var lifecycleService = new ApplicationLifecycleService(_logger);
        bool loaded = false;
        
        lifecycleService.ApplicationLoaded += (s, e) => loaded = true;

        // Act - signals arrive in different order
        lifecycleService.SignalApplicationInitialized();
        await Task.Delay(10);
        lifecycleService.SignalUiReady();

  // Assert
        Assert.IsTrue(loaded); // ? Still fired!
    }
}
```

## Migration Checklist

If you're updating an existing app:

- [ ] Remove `_isHandlingInitialAuthentication` flag and related logic
- [ ] Remove `MainThread.BeginInvokeOnMainThread` with delays
- [ ] Remove `Task.Delay(500)` or similar workarounds
- [ ] Replace `ApplicationLoadedMessage` with `ApplicationLifecycleService` event
- [ ] Move all post-load navigation to `ApplicationLoadedAsync`
- [ ] Ensure `InitializeApplicationAsync` calls `SignalApplicationInitialized()`
- [ ] Test on slow devices (emulator) to verify no timing issues
- [ ] Remove duplicate navigation code
- [ ] Update any custom ApplicationLifecycleService implementations

## Summary

The new architecture solves the core problem: **providing a reliable, single point in time when both UI and business logic are ready**, eliminating the need for flags, delays, and complex coordination logic.

Applications are now simpler, more reliable, and easier to maintain.
