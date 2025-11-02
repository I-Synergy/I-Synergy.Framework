# AuthenticationLifecycleService - Quick Reference

## What Changed

Replaced message-based authentication state changes with event-based lifecycle management.

## Quick Comparison

### Before (Message-Based)
```csharp
// App.xaml.cs
_commonServices.MessengerService.Register<AuthenticationChangedMessage>(this, 
    async m => await AuthenticationChangedAsync(m));

private async Task AuthenticationChangedAsync(AuthenticationChangedMessage msg)
{
    if (msg.Content is Profile profile)
        await _navigationService.NavigateModalAsync<ShellViewModel>();
 else
        await _navigationService.NavigateModalAsync<SignInViewModel>();
}
```

### After (Event-Based)
```csharp
// App.xaml.cs
var authService = _commonServices.ScopedContextService
    .GetRequiredService<IAuthenticationLifecycleService>();

authService.AuthenticationSucceeded += 
    async (s, e) => await OnAuthenticationSucceededAsync(e);
authService.AuthenticationFailed += 
    async (s, e) => await OnAuthenticationFailedAsync();

private async Task OnAuthenticationSucceededAsync(AuthenticationSuccessEventArgs e)
{
    await _navigationService.NavigateModalAsync<ShellViewModel>();
}

private async Task OnAuthenticationFailedAsync()
{
    await _navigationService.NavigateModalAsync<SignInViewModel>();
}
```

## Core API

```csharp
// Events
event EventHandler<AuthenticationSuccessEventArgs>? AuthenticationSucceeded;
event EventHandler<EventArgs>? AuthenticationFailed;

// Methods
void SignalAuthenticationSucceeded(bool shouldRemember = false);
void SignalAuthenticationFailed();

// Property
bool IsAuthenticated { get; }
```

## AuthenticationSuccessEventArgs

```csharp
public class AuthenticationSuccessEventArgs : EventArgs
{
    public bool ShouldRemember { get; }  // For "Remember Me" feature
}
```

## Usage in AuthenticationService

```csharp
private void ValidateToken(Token? token, bool shouldRemember = false)
{
    if (token is not null)
    {
        // Signal success
        _authenticationLifecycleService.SignalAuthenticationSucceeded(shouldRemember);
        
        // Legacy message (for backward compatibility)
        MessengerService.Default.Send(new AuthenticationChangedMessage(profile));
    }
    else
    {
  // Signal failure
        _authenticationLifecycleService.SignalAuthenticationFailed();
        
        // Legacy message (for backward compatibility)
        MessengerService.Default.Send(new AuthenticationChangedMessage(null));
    }
}
```

## Key Benefits

? **Type-safe**: Custom EventArgs instead of loose object messages  
? **Event-driven**: Clear, testable subscriptions  
? **Atomic**: Events fire exactly once, no duplicates  
? **Thread-safe**: Uses Interlocked operations  
? **Compatible**: Maintains legacy message support  
? **Resilient**: Exception handling doesn't crash service

## Event Flow

```
AuthenticationService.AuthenticateWithUsernamePasswordAsync()
    ?
ValidateToken(token, remember)
    ?
SignalAuthenticationSucceeded(remember)
    ?
AuthenticationSucceeded event fires
    ?
App.xaml.cs::OnAuthenticationSucceededAsync()
    ?
Navigate to Shell
```

## Common Patterns

### Check Authentication Status
```csharp
bool isLoggedIn = _authenticationLifecycleService.IsAuthenticated;
```

### Handle Auto-Login
```csharp
private async Task OnAuthenticationSucceededAsync(AuthenticationSuccessEventArgs e)
{
    if (e.ShouldRemember)
    {
        // Save credentials for next session
    _settingsService.LocalSettings.DefaultUser = username;
        _settingsService.LocalSettings.IsAutoLogin = true;
    }
    
    await _navigationService.NavigateModalAsync<ShellViewModel>();
}
```

### Subscribe in Constructor
```csharp
public App()
{
    var authService = GetRequiredService<IAuthenticationLifecycleService>();
    authService.AuthenticationSucceeded += async (s, e) => 
        await OnAuthenticationSucceededAsync(e);
    authService.AuthenticationFailed += async (s, e) => 
        await OnAuthenticationFailedAsync();
}
```

## Files Changed

| File | Change |
|------|--------|
| `src/ISynergy.Framework.UI/Abstractions/Services/IAuthenticationLifecycleService.cs` | ? New interface |
| `src/ISynergy.Framework.UI/Services/AuthenticationLifecycleService.cs` | ? New implementation |
| `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs` | ?? Added DI registration |
| `src/ISynergy.Framework.UI.Maui/Application/Application.cs` | ?? Added lifecycle field |
| `samples/Sample.Shared/Services/AuthenticationService.cs` | ?? Inject & signal lifecycle |
| `samples/Sample.Maui/App.xaml.cs` | ?? Subscribe to events |
| `samples/Sample.Shared/Sample.Shared.csproj` | ?? Added UI reference |

## Testing

Run unit tests:
```bash
dotnet test tests/ISynergy.Framework.UI.Tests/
```

## Migration Status

? Framework: Complete  
? Sample App: Complete  
? Tests: Included  
? Backward Compatibility: Maintained  
? Build: Successful

---

**Next Steps**: Remove legacy `AuthenticationChangedMessage` handlers once all code is migrated.
