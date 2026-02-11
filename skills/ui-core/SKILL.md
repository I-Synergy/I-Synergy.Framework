```skill
{
  "name": "i-synergy-ui-core",
  "description": "Cross-platform UI abstractions (authentication, token storage, localization, theme + language view models) that every ISynergy shell builds on.",
  "version": "2026.02.10",
  "sources": [
    "src/ISynergy.Framework.UI",
    "src/ISynergy.Framework.Mvvm",
    "docs/Theme-Color-Reference.md",
    "docs/MAUI-Dynamic-Theme-System.md"
  ],
  "license": "See license.md in repository root"
}
```

# I-Synergy Framework UI Core Skill (SKILL.md)

Use this skill whenever you need to wire up *platform-agnostic* UI services. The base UI package supplies the abstractions consumed by WPF, WinUI, UWP, MAUI, and Blazor shells.

---

## 1. Feature Matrix

| Capability | API | Notes |
| --- | --- | --- |
| Command/UI authorization | `IAuthenticationProvider` | Centralizes `CanExecute` and visibility rules for any shell; inject into DI and delegate to your auth service. |
| Secure token storage | `ITokenStorageService` | Async store/get/clear tokens; use for OAuth/JWT flows. |
| Localization | `ILanguageService`, `LanguageViewModel` | Provide language picker windows/dialogs across platforms. |
| Theme management | `ThemeViewModel`, `ThemeWindow` abstractions | Works with `IThemeService` implementations in each shell. |
| Splash screen config | `SplashScreenOptions` | Configure display duration/type via `IOptions`. |
| Resource registration | `services.RegisterAssemblies(...)` | Auto-register views/viewmodels/windows filtered by assembly predicate. |

See [src/ISynergy.Framework.UI/readme.md](src/ISynergy.Framework.UI/readme.md) for the canonical samples referenced below.

---

## 2. Authentication Provider Pattern

```csharp
using ISynergy.Framework.UI.Abstractions.Providers;
using Microsoft.Extensions.Logging;

public sealed class PortalAuthenticationProvider : IAuthenticationProvider
{
    private readonly IPortalAuthService _authService;
    private readonly ILogger<PortalAuthenticationProvider> _logger;

    public PortalAuthenticationProvider(
        IPortalAuthService authService,
        ILogger<PortalAuthenticationProvider> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public bool CanCommandBeExecuted(ICommand command, object parameter)
    {
        var requiredRole = command switch
        {
            AuthorizeCommand attr => attr.RequiredRole,
            _ => string.Empty
        };

        return string.IsNullOrEmpty(requiredRole) || _authService.CurrentUser.HasRole(requiredRole);
    }

    public bool HasAccessToUIElement(object element, object tag, string authorizationTag)
    {
        return string.IsNullOrEmpty(authorizationTag) || _authService.CurrentUser.HasPermission(authorizationTag);
    }
}

services.AddScoped<IAuthenticationProvider, PortalAuthenticationProvider>();
```

- Keep role/permission checks isolated here so WPF/WinUI/Blazor share identical logic.
- Inject the provider into command implementations or UI behaviors via DI.

---

## 3. Theme + Language Dialogs

```csharp
public sealed class SettingsViewModel : ViewModel
{
    public AsyncRelayCommand ChangeThemeCommand { get; }
    public AsyncRelayCommand ChangeLanguageCommand { get; }

    public SettingsViewModel(ICommonServices commonServices, ILogger<SettingsViewModel> logger)
        : base(commonServices, logger)
    {
        ChangeThemeCommand = new AsyncRelayCommand(ChangeThemeAsync);
        ChangeLanguageCommand = new AsyncRelayCommand(ChangeLanguageAsync);
    }

    private Task ChangeThemeAsync() =>
        CommonServices.DialogService.ShowDialogAsync<ThemeWindow, ThemeViewModel, ThemeStyle>();

    private Task ChangeLanguageAsync() =>
        CommonServices.DialogService.ShowDialogAsync<LanguageWindow, LanguageViewModel, Languages>();
}
```

- `ThemeWindow`/`LanguageWindow` are provided by platform packages; ViewModels come from this core layer.
- Persist selections via `ISettingsService` so other shells can read them on startup.

---

## 4. Token Storage

```csharp
public sealed class AuthTokens
{
    public static class Keys
    {
        public const string Access = "access_token";
        public const string Refresh = "refresh_token";
    }
}

public sealed class AuthService
{
    private readonly ITokenStorageService _tokenStorage;

    public AuthService(ITokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public async Task PersistAsync(TokenResponse dto)
    {
        await _tokenStorage.StoreTokenAsync(AuthTokens.Keys.Access, dto.AccessToken);
        await _tokenStorage.StoreTokenAsync(AuthTokens.Keys.Refresh, dto.RefreshToken);
    }

    public Task<string> GetAccessTokenAsync() =>
        _tokenStorage.GetTokenAsync(AuthTokens.Keys.Access);

    public Task LogoutAsync() => _tokenStorage.ClearAllTokensAsync();
}
```

- Always use the provided storage service (no plaintext `Preferences`/`LocalSettings`).
- Pair with `IAuthenticationProvider` to enforce token expiration policies.

---

## 5. Splash + Assembly Registration

```csharp
services.Configure<SplashScreenOptions>(options =>
{
    options.Type = SplashScreenTypes.Extended;
    options.DisplayDuration = TimeSpan.FromSeconds(3);
    options.MinimumDisplayTime = TimeSpan.FromSeconds(1);
});

var entryAssembly = Assembly.GetExecutingAssembly();
services.RegisterAssemblies(
    entryAssembly,
    asm => asm.Name!.StartsWith("MyCompany."));
```

- `RegisterAssemblies` scans for `View`, `ViewModel`, `Window`, and `Dialog` types.
- Keep filters narrow to avoid pulling in third-party assemblies that should not be auto-registered.

---

## 6. Extension Highlights

| Extension | Purpose |
| --- | --- |
| `Credential.ToBase64()` / `.FromBase64ToCredential()` | Securely serialize credentials for storage/telemetry. |
| `DateTimeOffset.ToLocalString(ILanguageService)` | Culture-aware formatting derived from the user’s language. |
| `decimal.ToCurrency(ILanguageService)` | Unified money formatting between shells. |
| `Languages.GetCulture()` / `.GetDescription()` | Build locale pickers or display names from enum values. |
| `Exception.Track()` | Pushes exceptions into configured OpenTelemetry exporters. |

---

## 7. Implementation Checklist

1. **Register** language, info, messenger, busy, token storage, and authentication services in DI.
2. **Provide** platform-specific dialog/navigation/theme services (from WPF/WinUI/MAUI/Blazor packages).
3. **Expose** `ThemeViewModel`/`LanguageViewModel` to UI for preference dialogs.
4. **Persist** preferences in `ISettingsService` and rehydrate during shell startup.
5. **Guard** commands and UI with `IAuthenticationProvider` instead of ad-hoc checks.
6. **Wire** telemetry by calling `.Track()` on exceptions or using the provided ActivitySource.

This skill should precede any platform-specific skill so all shells share identical authorization, localization, theme, and token flows.
