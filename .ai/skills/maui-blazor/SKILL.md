```skill
{
  "name": "i-synergy-maui-ui",
  "description": "I-Synergy MVVM patterns for .NET MAUI + Blazor Hybrid shells (shared ViewModels, ConfigureServices bootstrapper, BlazorWebView orchestration).",
  "version": "2026.02.10",
  "sources": [
    "samples/Sample.Maui",
    "src/ISynergy.Framework.UI.Maui",
    "src/ISynergy.Framework.Mvvm",
    "docs/MAUI-Dynamic-Theme-System.md"
  ],
  "license": "See license.md in repository root"
}
```

# I-Synergy Framework MAUI + Blazor Skill (SKILL.md)

**Target shells**: .NET MAUI (Android, iOS, macOS, Windows) using `BlazorWebView` + native pages.
**Objective**: Reuse the exact ViewModels, services, and design system that power the MAUI, Blazor, WinUI, and WPF samples.

---

## 1. Architecture Highlights

1. **Unified bootstrapper** – `builder.Services.ConfigureServices<Context, CommonServices, ExceptionHandlerService, SettingsService, Resources>(...)` is called inside `MauiProgram.CreateMauiApp()` exactly the same way as [samples/Sample.Blazor/Program.cs](samples/Sample.Blazor/Program.cs). This registers the MVVM stack, logging, localization, and dialog/navigation services.
2. **Shared ViewModels** – Pages declare `<c:View x:TypeArguments="viewmodels:DashboardViewModel" />` so the same ViewModel used by Blazor Razor components also powers MAUI XAML views. Review [samples/Sample.Maui/Views](samples/Sample.Maui/Views) and [samples/Sample.Maui/ViewModels](samples/Sample.Maui/ViewModels).
3. **BlazorWebView hosting** – When you need Fluent UI components or Razor UI inside MAUI, embed `<BlazorWebView>` with `RootComponents` pointing to the Razor component inheriting `View<TViewModel>` (see [samples/Sample.Maui/Views/BlazorHostPage.xaml](samples/Sample.Maui/Views/BlazorHostPage.xaml)).
4. **Theme + form factor services** – `IThemeService` (documented in [docs/MAUI-Dynamic-Theme-System.md](docs/MAUI-Dynamic-Theme-System.md)) syncs accent colors between WinUI, MAUI, and Fluent UI. `IFormFactorService` adapts breakpoints so ViewModels can react to device orientation.
5. **Context propagation** – `ScopedContextService` flows tenant/user data from the DI root into modal pages and dialogs, identical to the Blazor implementation.

---

## 2. Project Setup Template

```csharp
// MauiProgram.cs
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("Montserrat-SemiBold.ttf", "DisplayFont");
            fonts.AddFont("SpaceGrotesk-Regular.ttf", "BodyFont");
        });

    var infoService = new InfoService();
    infoService.LoadAssembly(typeof(App).Assembly);

    builder.Services
        .ConfigureServices<Context, CommonServices, ExceptionHandlerService, SettingsService, Properties.Resources>(
            builder.Configuration,
            infoService,
            services =>
            {
                services.AddMauiBlazorWebView();
#if DEBUG
                services.AddBlazorWebViewDeveloperTools();
#endif
                services.AddFluentUIComponents();
            },
            typeof(App).Assembly);

    builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
    builder.Services.AddSingleton<IFilePicker>(FilePicker.Default);

    return builder.Build();
}
```

> Keep the ConfigureServices call identical to the other shells so `View<TViewModel>` and the scoped context services behave the same.

---

## 3. Navigation Patterns

| Scenario | Pattern | Reference |
| --- | --- | --- |
| Navigate between native MAUI pages | `await Shell.Current.GoToAsync("//settings");` | [samples/Sample.Maui/AppShell.xaml](samples/Sample.Maui/AppShell.xaml) |
| Navigate within embedded Blazor UI | `NavigationManager.NavigateTo("/budgets");` | [samples/Sample.Blazor/Components/Pages](samples/Sample.Blazor/Components/Pages) |
| Launch dialog with shared ViewModel | `_dialogService.ShowDialogAsync<SampleDialog>(viewModel, options);` | [samples/Sample.Maui/ViewModels/Dialogs](samples/Sample.Maui/ViewModels) |
| Bridge MAUI -> Blazor command | Use `BlazorWebView.Dispatcher.DispatchAsync(() => command.Execute(parameter));` | [src/ISynergy.Framework.UI.Maui/Extensions/BlazorWebViewExtensions.cs](src/ISynergy.Framework.UI.Maui/Extensions/BlazorWebViewExtensions.cs) |

Keep navigation logic inside ViewModels so any shell (MAUI, WinUI, WPF, Blazor) can execute identical flows.

---

## 4. Platform Service Injection

```csharp
public interface IDeviceService
{
    string GetModel();
    Task<bool> EnsurePermissionAsync<TPermission>() where TPermission : Permissions.BasePermission, new();
}

public class DeviceService : IDeviceService
{
    public string GetModel() => DeviceInfo.Current.Model;

    public async Task<bool> EnsurePermissionAsync<TPermission>() where TPermission : Permissions.BasePermission, new()
    {
        var status = await Permissions.CheckStatusAsync<TPermission>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<TPermission>();
        }
        return status == PermissionStatus.Granted;
    }
}

// Registration
builder.Services.AddSingleton<IDeviceService, DeviceService>();
```

In Razor components hosted via `BlazorWebView`, inject `IDeviceService` and call its methods. Because ViewModels receive `ICommonServices`, they can request platform services by type from `ScopedContextService` without coupling to MAUI APIs.

---

## 5. ViewModel Reuse Checklist

- Derive from `ViewModel`, `ViewModelDialog<T>`, or `ViewModelNavigation<TParameter>` in `ISynergy.Framework.Mvvm`.
- Hold UI state using `GetValue<T>()/SetValue(value)` so property change notifications stay consistent across shells.
- Use `AsyncRelayCommand`/`RelayCommand` from CommunityToolkit.Mvvm; register them in the constructor; surface them to Razor views or XAML via bindings.
- Interact with dialogs using `IDialogService` and navigation using `INavigationService` instead of platform primitives.
- When you need UI-specific code (e.g., file pickers), wrap it in an abstraction registered in ConfigureServices and inject the abstraction into the ViewModel.

---

## 6. Design Direction Sync (shared with Blazor skill)

Adopt the same creative guidelines described in the consolidated Blazor skill:

- **Intentional aesthetic**: decide on a bold style (lux brutalism, retro grid, soft organic, etc.) that matches the page’s purpose.
- **Typography variables**: expose fonts via `App.xaml` resources and mirror them in `wwwroot/css/site.css` so both MAUI XAML and Blazor components share the same look.
- **Semantic colors**: define `Color` resources (`ColorSurface`, `ColorAccent`, `ColorInk`) and propagate them to Fluent tokens via `IThemeService`.
- **Motion**: use `Microsoft.Maui.Controls.Animation` for native elements and CSS keyframes for Blazor fragments, but keep sequencing consistent.
- **Accessibility**: maintain WCAG contrast, provide focus visuals for both XAML controls and Fluent components, honor reduced-motion preference from `AppThemeBinding`.

This keeps hybrid experiences visually cohesive regardless of whether the pixels are rendered by XAML or fluent Razor components.

---

## 7. Troubleshooting Cheatsheet

| Symptom | Likely Cause | Fix |
| --- | --- | --- |
| Blazor content not rendering inside MAUI | `RootComponents` missing or `HostPage` path incorrect | Match the Sample.Maui `BlazorWebView` setup; ensure `wwwroot/index.html` is copied to output |
| Commands don’t fire from MAUI button | Forgot to call `SetLocatorProvider()` or register `View<TViewModel>` | Confirm ConfigureServices + `SetLocatorProvider()` executed in `MauiProgram` |
| Dialog ViewModel cannot resolve services | `ScopedContextService` not initialized | Ensure `SetLocatorProvider()` called and dialogs created through `IDialogService` |
| Theme mismatch between MAUI and Blazor | `IThemeService` not notified of theme change | Call `_themeService.SetThemeAsync(theme)` and raise `ThemeChanged` so Fluent theme updates |
| Navigation stack desync | Mixing Shell navigation with manual `Navigation.PushAsync` | Standardize on Shell routes or wrap navigation in `INavigationService` |

---

## 8. When to Fall Back to Blazor Skill

Use this MAUI skill when the topic touches device APIs, navigation between native pages, performance on mobile, or BlazorWebView hosting. When you need pure Razor guidance (components, routing, Fluent UI details), jump to the consolidated [I-Synergy Framework Blazor Skill](../blazor-fluentui/SKILL.md) — both skills share the same templates and design rules.
