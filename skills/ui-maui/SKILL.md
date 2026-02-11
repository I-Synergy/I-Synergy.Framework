```skill
{
  "name": "i-synergy-ui-maui",
  "description": ".NET MAUI implementation of the I-Synergy UI stack: ConfigureServices bootstrapper, cross-platform dialog/navigation/theme/file/camera services, NavigationMenu controls, and dynamic theme system.",
  "version": "2026.02.10",
  "sources": [
    "src/ISynergy.Framework.UI.Maui",
    "samples/Sample.Maui",
    "docs/MAUI-Dynamic-Theme-System.md"
  ],
  "license": "See license.md in repository root"
}
```

# I-Synergy Framework UI MAUI Skill (SKILL.md)

Everything here points at the actual code under [src/ISynergy.Framework.UI.Maui](src/ISynergy.Framework.UI.Maui). Use these sections when wiring MAUI apps to the framework services instead of relying on the readme.

---

## 1. `MauiAppBuilderExtensions.ConfigureServices`

`ConfigureServices<TApplication, TContext, TCommonServices, TExceptionHandlerService, TSettingsService, TResource>` in [src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs](src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs) is the single entry point for DI:

```csharp
appBuilder.Services.Configure<ApplicationFeatures>(appBuilder.Configuration.GetSection(nameof(ApplicationFeatures)).BindWithReload);
appBuilder.Services.Configure<ClientApplicationOptions>(appBuilder.Configuration.GetSection(nameof(ClientApplicationOptions)).BindWithReload);

appBuilder.Services.TryAddSingleton<IInfoService>(s => InfoService.Default);
appBuilder.Services.TryAddSingleton<ILanguageService>(s => languageService);
appBuilder.Services.TryAddSingleton<IMessengerService, MessengerService>();
appBuilder.Services.TryAddSingleton<IPreferences>(s => Preferences.Default);
appBuilder.Services.TryAddSingleton<IMigrationService, MigrationService>();

appBuilder.Services.TryAddSingleton<TContext>();
appBuilder.Services.TryAddSingleton<IContext>(s => s.GetRequiredService<TContext>());
appBuilder.Services.TryAddSingleton<IExceptionHandlerService, TExceptionHandlerService>();
appBuilder.Services.TryAddScoped<ISettingsService, TSettingsService>();
appBuilder.Services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();

appBuilder.Services.TryAddSingleton<IApplicationLifecycleService, ApplicationLifecycleService>();
appBuilder.Services.TryAddSingleton<INavigationService, NavigationService>();
appBuilder.Services.TryAddSingleton<IDialogService, DialogService>();
appBuilder.Services.TryAddSingleton<IThemeService, ThemeService>();
appBuilder.Services.TryAddSingleton<IClipboardService, ClipboardService>();
appBuilder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();
```

- `RegisterAssemblies(assembly, assemblyFilter)` pulls every `View`, `ViewModel`, and service in the calling assembly into DI.
- `AddPageResolver()` registers `MauiInitializerService`, ensuring each page created through the shell resolves its scoped services before appearing.
- Fonts are configured right in this extension; you get the Segoe/Open Sans bundle unless you override it.

---

## 2. Logging + OpenTelemetry

`ConfigureLogging` in the same file wires `ILogger` + OpenTelemetry ([src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs](src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs)):

- `AddOpenTelemetry()` is called for both tracing and metrics with HTTP and runtime instrumentation enabled.
- Debug builds automatically add `AddConsoleExporter()` so you can see spans/metrics while iterating.
- Override the optional `Action<TracerProviderBuilder>` / `Action<MeterProviderBuilder>` parameters to add exporters (OTLP, Azure Monitor, etc.) without touching the framework code.

---

## 3. Dynamic theme pipeline

- `ApplicationExtensions.SetApplicationColor` ([src/ISynergy.Framework.UI.Maui/Extensions/ApplicationExtensions.cs](src/ISynergy.Framework.UI.Maui/Extensions/ApplicationExtensions.cs)) swaps every `ISynergy.Framework.UI.Resources.Styles.Themes` resource dictionary based on a hex string and falls back to `Themeffb900` if the color is unknown.
- `ThemeService` ([src/ISynergy.Framework.UI.Maui/Services/ThemeService.cs](src/ISynergy.Framework.UI.Maui/Services/ThemeService.cs)) pulls `Theme`/`Color` from `ISettingsService`, calls the extension, sets `Application.AccentColor`, and updates platform resources (Android colors, Windows title bar) inside `UpdatePlatformColors()`.
- The service guards `Application.Current` nullability, so you can call `IThemeService.ApplyTheme()` any time after the app builds.

---

## 4. NavigationService behavior

The MAUI implementation in [src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs](src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs) wraps `Page.GetNavigation()` and drives both stack and modal navigation:

- `NavigateAsync<TViewModel>` uses `NavigationExtensions.CreatePage<TViewModel>` to construct the view, pushes it if it is not already on the stack, and calls `InitializeAsync()` on the ViewModel with centralized exception handling.
- `NavigateModalAsync<TViewModel>` swaps `Application.Current.Windows[0].Page` and reruns initialization; dispatcher-aware logic re-enters on the UI thread if needed.
- `CanGoBack` inspects both navigation and modal stacks. `CleanBackStackAsync` pops to root.
- Blade APIs (`OpenBladeAsync` etc.) throw `[Obsolete("Not supported!", true)]` so you do not accidentally call desktop-only features.

---

## 5. Platform services bundled in DI

`ConfigureServices` automatically registers:

| Service | Implementation | Notes |
| --- | --- | --- |
| `ITokenStorageService` | [TokenStorageService](src/ISynergy.Framework.UI.Maui/Services/TokenStorageService.cs) | Secure tokens per platform via `SecureStorage`. |
| `IDialogService` | [DialogService](src/ISynergy.Framework.UI.Maui/Services/DialogService.cs) | Presents modal pages/windows consistently. |
| `IFileService<FileResult>` | [FileService](src/ISynergy.Framework.UI.Maui/Services/FileService.cs) | Uses `FilePicker`/`FileSaver` across OSs. |
| `ICameraService` | [CameraService](src/ISynergy.Framework.UI.Maui/Services/CameraService.cs) | Wraps `MediaPicker`; request permissions before calling. |
| `IClipboardService` | [ClipboardService](src/ISynergy.Framework.UI.Maui/Services/ClipboardService.cs) | Maps to `Clipboard.Default`. |

You get `IApplicationLifecycleService`, `IDispatcherService`, and `IScopedContextService` out of the box so background operations and ViewModel scopes behave exactly like WPF/WinUI.

---

## 6. Page resolver + initialization

`AddPageResolver()` registers `IMauiInitializeService` ([src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs](src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs)), pointing at `MauiInitializerService`. That initializer resolves each page through DI, so every `ui:View` automatically receives its ViewModel and `CommonServices` before hitting the visual tree. No manual service locator calls are needed.

---

## 7. Troubleshooting directly from source

| Symptom | Actual cause in code | Fix |
| --- | --- | --- |
| `NavigationService` throws `InvalidOperationException("Main page is not available.")` | `GetMainPage()` (NavigationService) could not find `Application.Current.Windows[0].Page`. | Ensure you set a `Page` when the app launches (e.g., `MainPage = new AppShell();`). |
| Theme never updates | `ThemeService.ApplyTheme()` exits when `Application.Current` is null. | Call the service after `builder.Build()` and `app.Run()` or from `MauiProgram` right after you set `MainPage`. |
| Fonts missing | You removed the bundled font registrations in `ConfigureServices`. | Re-add `ConfigureFonts` call or add your own fonts before calling `UseMauiApp<App>()`. |
| File picker returns empty string | `StaticAssetService` returns `string.Empty` when `HttpClient.SendAsync` fails. | Inspect HTTP response; the service intentionally swallows errors to let ViewModels decide how to react. |

Pair this MAUI skill with **ui-core** for shared auth/context logic and **ui-blazor** if you embed Razor components via `BlazorWebView`.
