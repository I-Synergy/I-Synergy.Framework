# Demo Authentication Service - Implementation Guide

## Overview

Created a comprehensive **fake/demo AuthenticationService** for sample applications that:

- ? Accepts **any username/password combination** for testing
- ? Creates **realistic profile data** with demo values
- ? **Generates mock tokens** for testing
- ? **Supports "Remember Me"** functionality
- ? **Raises events** for authentication state changes
- ? **Full event-driven architecture** (no legacy messages)

## Key Features

### 1. Accepts Any Credentials
```csharp
// Any username and password are accepted
await authService.AuthenticateWithUsernamePasswordAsync("testuser", "anypassword", remember: true);
await authService.AuthenticateWithUsernamePasswordAsync("admin", "password123", remember: false);
await authService.AuthenticateWithUsernamePasswordAsync("john.doe", "secret", remember: true);
```

### 2. Creates Realistic Demo Profile

For any login attempt, creates a profile with:

```csharp
var profile = new Profile
{
    Token = GeneratedMockToken,        // Mock JWT-like tokens
 AccountId = "{79C13C79-B50B-...}",   // Fixed demo account ID
    Description = "Demo Account - {username}",
    TimeZoneId = "Europe/Amsterdam",      // Default timezone
    CountryCode = "NL",                   // Default country
    CultureCode = "nl-NL",        // Default culture
    UserId = Guid.NewGuid(),              // Unique per login
Username = username,            // From login form
    Email = "{username}@demo.com",        // Generated from username
  Roles = ["Administrator", "User"],    // Demo roles
    Modules = ["Dashboard", "Settings", "Admin"],  // Demo modules
    Expiration = DateTimeOffset.Now.AddHours(24)  // 24-hour validity
};
```

### 3. Event-Driven Architecture

```csharp
// Subscribe to authentication events
authService.AuthenticationSucceeded += async (s, e) =>
{
    _logger.LogInformation("User logged in successfully");
    if (e.ShouldRemember)
        _logger.LogInformation("Remember-me is enabled");
};

authService.AuthenticationFailed += async (s, e) =>
{
    _logger.LogInformation("Authentication failed");
};
```

### 4. Mock Token Generation

```csharp
private string GenerateToken()
{
    // Generates base64-encoded GUID strings (not real JWT)
    // Format: [base64(guid1)][base64(guid2)]
  return Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + 
           Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}
```

Produces tokens like: `ALoSvP3kUUe1E...c/Q==MjM1NDU1...T8zQ==`

### 5. Remember-Me Functionality

```csharp
// Save default user for auto-login
if (remember)
{
    settingsService.LocalSettings.IsAutoLogin = true;
    settingsService.LocalSettings.DefaultUser = username;
    settingsService.SaveLocalSettings();
}
```

## Testing Scenarios

### Scenario 1: Simple Login
```csharp
await authService.AuthenticateWithUsernamePasswordAsync("testuser", "password", remember: false);
// Result: User authenticated, profile set, event raised
```

### Scenario 2: Login with Remember-Me
```csharp
await authService.AuthenticateWithUsernamePasswordAsync("admin", "secret", remember: true);
// Result: User authenticated, credentials saved for auto-login
```

### Scenario 3: Admin User
```csharp
await authService.AuthenticateWithUsernamePasswordAsync("admin", "any", remember: false);
// Result: Profile with "Administrator" role
```

### Scenario 4: Logout
```csharp
await authService.SignOutAsync();
// Result: Context cleared, AuthenticationFailed event raised
```

## Implementation Details

### Event Raising
```csharp
private void ValidateToken(Token? token, bool shouldRemember = false)
{
    if (token is not null)
    {
    IsAuthenticated = true;
        RaiseAuthenticationSucceeded(shouldRemember);
    }
    else
    {
      IsAuthenticated = false;
   RaiseAuthenticationFailed();
    }
}
```

### Exception Handling
```csharp
private void RaiseAuthenticationSucceeded(bool shouldRemember)
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
```

## Profile Data Reference

### Fixed Demo Values
| Property | Value | Purpose |
|----------|-------|---------|
| AccountId | `{79C13C79-B50B-4BEF-B796-294DED5676BB}` | Consistent demo account |
| TimeZoneId | `Europe/Amsterdam` | Default timezone |
| CountryCode | `NL` | Default country |
| CultureCode | `nl-NL` | Default culture |
| Roles | `["Administrator", "User"]` | Demo permissions |
| Modules | `["Dashboard", "Settings", "Admin"]` | Available modules |
| Expiration | `Now + 24 hours` | Token validity period |

### Dynamic Values
| Property | Source | Example |
|----------|--------|---------|
| Username | Login form | `admin`, `testuser`, `john.doe` |
| Email | Generated from username | `admin@demo.com` |
| UserId | `Guid.NewGuid()` | Unique per login |
| Description | Template with username | `Demo Account - admin` |
| Tokens | Generated tokens | Mock JWT-like strings |

## Production vs Demo Comparison

| Aspect | Production Service | Demo Service |
|--------|-------------------|--------------|
| **Credentials** | Validated against backend | Any accepted |
| **Token Source** | Real OAuth2/JWT server | Generated mock tokens |
| **Profile Data** | From identity server | Hardcoded demo data |
| **Security** | Full OWASP security | DEMO ONLY |
| **Purpose** | Real authentication | Testing/development |

## Integration with Sample Applications

### MAUI Sample
```csharp
var authService = _commonServices.ScopedContextService
    .GetRequiredService<IAuthenticationService>();

authService.AuthenticationSucceeded += async (s, e) =>
    await _navigationService.NavigateModalAsync<ShellViewModel>();
```

### WPF Sample
```csharp
protected override void OnStartup(StartupEventArgs e)
{
    var authService = _commonServices.ScopedContextService
        .GetRequiredService<IAuthenticationService>();
    
    authService.AuthenticationSucceeded += 
        async (s, args) => await OnAuthenticationSucceededAsync(args);
    
    RaiseApplicationLoaded();
}
```

### WinUI Sample
```csharp
var authService = _commonServices.ScopedContextService
    .GetRequiredService<IAuthenticationService>();

authService.AuthenticationSucceeded += 
    async (s, args) => await OnAuthenticationSucceededAsync(args);
```

## Quick Start Testing

### Test Login Scenarios
```csharp
// Scenario 1: Admin login
var success = await authService.AuthenticateWithUsernamePasswordAsync(
    "admin", "password", remember: false);
// Result: IsAuthenticated = true, AuthenticationSucceeded event raised

// Scenario 2: User login with remember-me
await authService.AuthenticateWithUsernamePasswordAsync(
    "john.doe", "secret", remember: true);
// Result: Settings saved for auto-login

// Scenario 3: Logout
await authService.SignOutAsync();
// Result: IsAuthenticated = false, AuthenticationFailed event raised
```

### Test Event Handling
```csharp
var eventFired = false;

authService.AuthenticationSucceeded += (s, e) =>
{
    eventFired = true;
Assert.IsTrue(e.ShouldRemember); // If remember=true was passed
};

await authService.AuthenticateWithUsernamePasswordAsync("test", "pwd", true);

Assert.IsTrue(eventFired);
```

## Security Warning

?? **IMPORTANT**: This is a **DEMO SERVICE ONLY** for development and testing.

**DO NOT** use this in production. The production service is located at:
- `..\I-Synergy\src\ISynergy.Services\Services\AuthenticationService.cs`

The production service implements:
- ? Real OAuth 2.0 with PKCE
- ? Secure token storage
- ? Biometric authentication
- ? Token refresh rotation
- ? Security audit logging
- ? OWASP mobile security best practices

## Features Intentionally Not Implemented

These production features are **not** in the demo service:

| Feature | Reason |
|---------|--------|
| Credential validation | Any credentials accepted for testing |
| Real token generation | Mock tokens for demo use |
| Secure storage | No secure token storage |
| Biometric auth | Not needed for desktop testing |
| Token refresh | Tokens valid for fixed 24 hours |
| Security audit | Minimal logging for demo |

## Usage Recommendations

### For Unit Testing
```csharp
[TestMethod]
public async Task LoginSuccessful_NavigatesToShell()
{
    var authService = new AuthenticationService(_context, _logger);
    var navigationCalled = false;
    
    authService.AuthenticationSucceeded += (s, e) => 
        navigationCalled = true;
    
    await authService.AuthenticateWithUsernamePasswordAsync("test", "pwd", false);
    
    Assert.IsTrue(navigationCalled);
}
```

### For Manual Testing
```csharp
// Test login form
await authService.AuthenticateWithUsernamePasswordAsync(
    enteredUsername, 
    enteredPassword, 
rememberCheckbox.IsChecked);

// Any username/password works - focus on UI flow testing
```

### For Integration Testing
```csharp
// Test complete authentication flow
await authService.AuthenticateWithUsernamePasswordAsync("user1", "pass1", true);
var profile1 = _context.Profile;

await authService.SignOutAsync();
var profile2 = _context.Profile;

Assert.IsNotNull(profile1);
Assert.IsNull(profile2);
```

## Summary

The demo AuthenticationService provides a **complete, flexible testing environment** for sample applications with:

- ? **Zero credential validation** - any username/password accepted
- ? **Realistic profile generation** - proper demo data structure
- ? **Event-driven architecture** - full async/await support
- ? **Remember-me support** - settings persistence
- ? **Mock tokens** - JWT-like token generation
- ? **Exception handling** - safe event raising
- ? **Full logging** - trace-level diagnostics

Perfect for development, testing, and demonstration purposes!

---

**?? Remember: Never use in production. This is DEMO ONLY.**
