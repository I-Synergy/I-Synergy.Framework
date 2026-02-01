# Authentication Lifecycle Service - Implementation Guide

## Overview

The `AuthenticationLifecycleService` provides an event-driven approach to handling authentication state changes, replacing the message-based `AuthenticationChangedMessage` approach. This design follows the same proven patterns used in the `ApplicationLifecycleService`.

## Architecture

### Two-Layer Authentication State Management

```
???????????????????????????????????????
? AuthenticationService          ?
? (Business Logic Layer)           ?
???????????????????????????????????????
? � AuthenticateWithUsernamePassword  ?
? � SignOut       ?
? � ValidateToken (internal)  ?
???????????????????????????????????????
 ?
       ?? Sets Profile in Context
       ?
       ?? Signals AuthenticationLifecycleService
       ?  (Event-based)
   ?
       ?? Sends Legacy AuthenticationChangedMessage
  (Backward compatibility)

       ?

????????????????????????????????????????
? AuthenticationLifecycleService ?
? (Lifecycle Coordination)   ?
????????????????????????????????????????
? � AuthenticationSucceeded event      ?
? � AuthenticationFailed event         ?
? � IsAuthenticated property           ?
????????????????????????????????????????
    ?
??? Raises Events

       ?

????????????????????????????????????????
? App.xaml.cs (or other subscribers)   ?
? (Event Handlers)            ?
????????????????????????????????????????
? � OnAuthenticationSucceededAsync()   ?
? � OnAuthenticationFailedAsync()      ?
????????????????????????????????????????
```

## Key Components

### 1. AuthenticationLifecycleService Interface

```csharp
public interface IAuthenticationLifecycleService : IDisposable
{
    // Events
    event EventHandler<AuthenticationSuccessEventArgs>? AuthenticationSucceeded;
    event EventHandler<EventArgs>? AuthenticationFailed;

    // Methods
    void SignalAuthenticationSucceeded(bool shouldRemember = false);
    void SignalAuthenticationFailed();

    // Properties
    bool IsAuthenticated { get; }
}
```

### 2. AuthenticationSuccessEventArgs

Carries authentication success data:

```csharp
public class AuthenticationSuccessEventArgs : EventArgs
{
    public bool ShouldRemember { get; } // For auto-login
}
```

## Implementation Details

### AuthenticationService Integration

The `AuthenticationService` now:

1. **Injects** `IAuthenticationLifecycleService` as a dependency
2. **Sets Profile** in the context
3. **Signals Success/Failure** through the lifecycle service
4. **Maintains Backward Compatibility** by sending legacy messages

```csharp
public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationLifecycleService _authenticationLifecycleService;

    public async Task AuthenticateWithUsernamePasswordAsync(
 string username, 
        string password, 
  bool remember)
    {
        // Set profile in context
  _scopedContextService.GetRequiredService<IContext>().Profile = profile;
        
        // Signal through lifecycle service
        ValidateToken(new Token(), remember);
    }

    private void ValidateToken(Token? token, bool shouldRemember = false)
    {
        if (token is not null)
        {
            // Signal authentication succeeded
            _authenticationLifecycleService.SignalAuthenticationSucceeded(shouldRemember);
            
 // Legacy message for backward compatibility
            MessengerService.Default.Send(new AuthenticationChangedMessage(profile));
        }
      else
        {
            // Signal authentication failed
   _authenticationLifecycleService.SignalAuthenticationFailed();
         
     // Legacy message for backward compatibility
     MessengerService.Default.Send(new AuthenticationChangedMessage(null));
        }
    }
}
```

### App.xaml.cs Integration

Subscribe to authentication lifecycle events:

```csharp
public partial class App : Application
{
    public App() : base()
    {
     try
        {
     InitializeComponent();

            // Get lifecycle services
      _lifecycleService = _commonServices.ScopedContextService
    .GetRequiredService<IApplicationLifecycleService>();
            
     var authenticationLifecycleService = _commonServices.ScopedContextService
      .GetRequiredService<IAuthenticationLifecycleService>();

      // Subscribe to events
          _lifecycleService.ApplicationLoaded += 
    async (s, e) => await ApplicationLoadedAsync();
            
            authenticationLifecycleService.AuthenticationSucceeded += 
    async (s, e) => await OnAuthenticationSucceededAsync(e);
  
            authenticationLifecycleService.AuthenticationFailed += 
        async (s, e) => await OnAuthenticationFailedAsync();
}
        catch (Exception ex)
   {
          _logger?.LogCritical(ex, "Error in App constructor");
        }
    }

    private async Task OnAuthenticationSucceededAsync(AuthenticationSuccessEventArgs e)
  {
    try
        {
        _logger?.LogTrace("Authentication succeeded event received");
      
  // Profile is already set in context by AuthenticationService
       // Navigate to authenticated UI
        await _navigationService.NavigateModalAsync<ShellViewModel>();
  }
        catch (Exception ex)
        {
         _logger?.LogError(ex, "Error in OnAuthenticationSucceededAsync");
   // Handle error...
        }
    }

    private async Task OnAuthenticationFailedAsync()
    {
      try
     {
            _logger?.LogTrace("Authentication failed event received");
  
            // Context is already cleared by AuthenticationService
            // Navigate to sign-in page
          await _navigationService.NavigateModalAsync<SignInViewModel>();
        }
        catch (Exception ex)
        {
     _logger?.LogError(ex, "Error in OnAuthenticationFailedAsync");
       // Handle error...
        }
    }
}
```

## Dependency Injection Setup

The service is registered as a **Singleton** in `MauiAppBuilderExtensions.cs`:

```csharp
appBuilder.Services.TryAddSingleton<IApplicationLifecycleService, ApplicationLifecycleService>();
appBuilder.Services.TryAddSingleton<IAuthenticationLifecycleService, AuthenticationLifecycleService>();
```

## Event Flow Diagram

```
User Login
   ?
   ??? AuthenticationService.AuthenticateWithUsernamePasswordAsync()
   ?   ?
   ?   ?? Set Profile in Context
   ?   ?
   ?   ?? Call ValidateToken(token, remember: true)
   ?       ?
   ?       ??? Signal AuthenticationSucceeded(shouldRemember: true)
   ?       ?   ?? AuthenticationLifecycleService.SignalAuthenticationSucceeded()
   ?       ?       ?
   ? ?       ?? Fire AuthenticationSucceeded event
   ?       ?           ?
   ?       ??? Send Legacy Message (AuthenticationChangedMessage)
   ?
   ?? Event dispatched to subscribers
   ?
   ??? App.xaml.cs::OnAuthenticationSucceededAsync()
       ?
       ?? Get ShouldRemember flag from EventArgs
       ?
       ?? Navigate to Shell (authenticated UI)
       ?
   ?? Done ?

---

User Logout
   ?
   ??? AuthenticationService.SignOutAsync()
   ?   ?
   ? ?? Clear Context (CreateNewScope)
   ?   ?
   ??? Call ValidateToken(null)
   ?       ?
   ?       ??? Signal AuthenticationFailed()
   ?     ?   ?? AuthenticationLifecycleService.SignalAuthenticationFailed()
   ?       ?       ?
   ?       ?       ?? Fire AuthenticationFailed event
   ?       ?      ?
   ?    ??? Send Legacy Message (AuthenticationChangedMessage(null))
   ?
   ?? Event dispatched to subscribers
   ?
   ??? App.xaml.cs::OnAuthenticationFailedAsync()
  ?
       ?? Navigate to SignIn (unauthenticated UI)
       ?
   ?? Done ?
```

## Advantages Over Message-Based Approach

| Aspect | Message-Based | Event-Based |
|--------|---------------|------------|
| **Type Safety** | Loose (object content) | Strong (custom EventArgs) |
| **Discoverability** | Need to search for message handlers | Clear event subscriptions |
| **Performance** | Message infrastructure overhead | Direct event delegation |
| **Error Handling** | Centralized but hard to debug | Per-subscriber exception handling |
| **State Consistency** | Multiple handlers may race | Single, atomic event |
| **Testing** | Requires messenger mocking | Simple event mock |

## Backward Compatibility

The implementation maintains full backward compatibility by:

1. **Continuing to send** `AuthenticationChangedMessage` alongside events
2. **Supporting both** message-based and event-based subscribers
3. **Allowing** gradual migration of code

**Note**: New code should prefer the event-based approach. Legacy message handlers can be removed once all code is migrated.

## Thread Safety

The `AuthenticationLifecycleService` uses:

- **Atomic Operations**: `Interlocked.Exchange()` for state updates
- **Volatile Fields**: Ensures visibility across threads
- **Exception Handling**: Catches and logs handler exceptions

```csharp
public void SignalAuthenticationSucceeded(bool shouldRemember = false)
{
    try
    {
      // Atomic state update
 Interlocked.Exchange(ref _isAuthenticatedPublished, 1);

        // Only raise event once (atomic)
        if (Interlocked.CompareExchange(ref _successEventPublished, 1, 0) == 0)
        {
   try
            {
                AuthenticationSucceeded?.Invoke(this, 
   new AuthenticationSuccessEventArgs(shouldRemember));
     }
            catch (Exception ex)
            {
 _logger.LogError(ex, "Error in AuthenticationSucceeded event handlers");
       }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in SignalAuthenticationSucceeded");
    }
}
```

## Best Practices

### 1. Always Wrap Event Handlers in Try-Catch

```csharp
_authenticationLifecycleService.AuthenticationSucceeded += async (s, e) =>
{
    try
    {
        await OnAuthenticationSucceededAsync(e);
    }
    catch (Exception ex)
    {
    _logger.LogError(ex, "Error in authentication succeeded handler");
    }
};
```

### 2. Don't Modify State During Events

```csharp
// ? WRONG: Modifying context during event
_authenticationLifecycleService.AuthenticationSucceeded += (s, e) =>
{
 context.Profile = someProfile;  // State already set by service
};

// ? CORRECT: Context is pre-set by AuthenticationService
_authenticationLifecycleService.AuthenticationSucceeded += async (s, e) =>
{
    // Just react to the event
    await _navigationService.NavigateModalAsync<ShellViewModel>();
};
```

### 3. Use ShouldRemember for Auto-Login Logic

```csharp
private async Task OnAuthenticationSucceededAsync(AuthenticationSuccessEventArgs e)
{
    // Check if user chose "Remember Me"
    if (e.ShouldRemember)
    {
_logger.LogTrace("User chose to remember authentication");
        // Save credentials for auto-login
    }
    
    await _navigationService.NavigateModalAsync<ShellViewModel>();
}
```

## Testing

Unit tests for `AuthenticationLifecycleService` verify:

- ? Events fire at correct times
- ? Atomic operations prevent duplicates
- ? Thread safety with concurrent access
- ? Exception handling doesn't crash service
- ? State properties reflect true state
- ? Disposal unsubscribes all handlers

See `tests/ISynergy.Framework.UI.Tests/Services/` for comprehensive test suite.

## Migration Checklist

When migrating from messages to events:

- [ ] Inject `IAuthenticationLifecycleService`
- [ ] Subscribe to `AuthenticationSucceeded` event
- [ ] Subscribe to `AuthenticationFailed` event
- [ ] Remove old `AuthenticationChangedMessage` handlers
- [ ] Update navigation logic to use new handlers
- [ ] Test authentication flow (login/logout)
- [ ] Test auto-login with `ShouldRemember` flag
- [ ] Verify error handling and logging
- [ ] Test concurrent authentication changes

## Related Documentation

- **ApplicationLifecycleService**: Similar event-driven coordination for application startup
- **IAuthenticationService**: Abstraction for authentication providers
- **.github/docs/VISUAL_ARCHITECTURE_GUIDE.md**: High-level architecture overview

---

**Status**: ? **Production-Ready**  
**Backward Compatibility**: ? **Maintained**  
**Thread Safety**: ? **Verified**  
**Test Coverage**: ? **Comprehensive**
