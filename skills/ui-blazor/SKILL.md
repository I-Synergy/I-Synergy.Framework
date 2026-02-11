```skill
{
  "name": "i-synergy-ui-blazor-library",
  "description": "Blazor UI library implementation (View<TViewModel>, ConfigureServices bootstrapper, Fluent UI integration, form-factor + antiforgery services, HTTP resilience).",
  "version": "2026.02.10",
  "sources": [
    "src/ISynergy.Framework.UI.Blazor",
    "samples/Sample.Blazor",
    "docs/IndicatorControl.md"
  ],
  "license": "See license.md in repository root"
}
```

# I-Synergy Framework UI Blazor Library Skill (SKILL.md)

Focus on the actual implementation under [src/ISynergy.Framework.UI.Blazor](src/ISynergy.Framework.UI.Blazor). Each section cites concrete types and behavior taken directly from the library.

---

## 1. Bootstrap pipeline (ConfigureServices)

`ServiceCollectionExtensions.ConfigureServices` in [src/ISynergy.Framework.UI.Blazor/Extensions/ServiceCollectionExtensions.cs](src/ISynergy.Framework.UI.Blazor/Extensions/ServiceCollectionExtensions.cs) wires the entire Blazor stack:

```csharp
services.Configure<ClientApplicationOptions>(configuration.GetSection(nameof(ClientApplicationOptions)).BindWithReload);
services.Configure<AnalyticOptions>(configuration.GetSection(nameof(AnalyticOptions)).BindWithReload);

var languageService = new LanguageService();
languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
languageService.AddResourceManager(typeof(TResource));

services.TryAddSingleton<IInfoService>(s => infoService);
services.TryAddSingleton<ILanguageService>(s => languageService);
services.TryAddSingleton<IMessengerService, MessengerService>();
services.TryAddSingleton<IScopedContextService, ScopedContextService>();
services.TryAddSingleton<IBusyService, BusyService>();

services.TryAddScoped<TContext>();
services.TryAddScoped<IContext>(s => s.GetRequiredService<TContext>());
services.TryAddSingleton<IExceptionHandlerService, TExceptionHandlerService>();
services.TryAddScoped<ISettingsService, TSettingsService>();
services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();
services.TryAddSingleton<TCommonServices>();
services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<TCommonServices>());

services.AddAuthorizationCore();
services.AddCascadingAuthenticationState();
services.AddHttpClient<IStaticAssetService, StaticAssetService>();
services.TryAddTransient<IAntiforgeryHttpClientFactory, AntiforgeryHttpClientFactory>();
services.TryAddSingleton<IFormFactorService, FormFactorService>();
services.TryAddSingleton<RequestCancellationService>();

services.RegisterAssemblies(assembly, assemblyFilter);
action.Invoke(services);
```

- The generic signature enforces application-specific context (`TContext : IContext`), common services, exception handling, and settings infrastructure.
- Language/translation support is composed by stacking resource managers for MVVM, UI, and caller-provided assemblies before registering `ILanguageService`.
- Static asset access, antiforgery handling, and form-factor detection are injected once and shared across components via DI.

---

## 2. MVVM-ready `View<TViewModel>` component

[src/ISynergy.Framework.UI.Blazor/Components/Controls/View.cs](src/ISynergy.Framework.UI.Blazor/Components/Controls/View.cs) implements a scoped Blazor component that binds a DI-provided ViewModel:

```razor
@inherits ComponentBase

@code {
    [Inject] public required TViewModel ViewModel { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!ViewModel.IsInitialized)
            await ViewModel.InitializeAsync();

        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        SubscribeToViewModelCommands();
    }

    private void SubscribeToViewModelCommands()
    {
        var commandProperties = ViewModel.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(ICommand).IsAssignableFrom(p.PropertyType));

        foreach (var property in commandProperties)
        {
            if (property.GetValue(ViewModel) is ICommand command)
            {
                command.CanExecuteChanged += OnCommandCanExecuteChanged;
                _subscribedCommands.Add(command);
            }
        }
    }
}
```

- Any ViewModel deriving from `IViewModel` is resolved, initialized once, and disposed when the component is destroyed.
- The helper stores every discovered `ICommand` property and refreshes the UI whenever `CanExecute` changes, giving native MVVM command semantics inside Razor components.

---

## 3. Command helpers for Razor components

`ComponentExtensions` in [src/ISynergy.Framework.UI.Blazor/Extensions/ComponentExtensions.cs](src/ISynergy.Framework.UI.Blazor/Extensions/ComponentExtensions.cs) exposes two helpers:

- `CommandBinding(component, command, parameter, disabled)` returns attribute dictionaries wiring `onclick` and `disabled` so a `<FluentButton @attributes="@this.CommandBinding(ViewModel.SaveCommand)" />` stays in sync with `CanExecute`.
- `CommandClick(component, command, parameter)` yields an `EventCallback<MouseEventArgs>` for direct binding when you do not need to control other attributes.

Both helpers call `command.Execute(parameter)` only when `CanExecute` returns true, preventing accidental double submits.

---

## 4. Form factor reporting

`FormFactorService` ([src/ISynergy.Framework.UI.Blazor/Services/FormFactorService.cs](src/ISynergy.Framework.UI.Blazor/Services/FormFactorService.cs)) currently reports a constant form factor:

```csharp
public class FormFactorService : IFormFactorService
{
    public string GetFormFactor() => "Web";
    public string GetPlatform() => Environment.OSVersion.ToString();
}
```

- The abstraction `IFormFactorService` lives under [src/ISynergy.Framework.UI.Blazor/Abstractions/Services/IFormFactorService.cs](src/ISynergy.Framework.UI.Blazor/Abstractions/Services/IFormFactorService.cs); swap this implementation to push richer device data through the same DI contract without touching consumers.

---

## 5. Static asset caching via HttpClient + cache storage

`StaticAssetService` ([src/ISynergy.Framework.UI.Blazor/Services/StaticAssetService.cs](src/ISynergy.Framework.UI.Blazor/Services/StaticAssetService.cs)) is injected wherever `IStaticAssetService` is requested:

- Accepts the ambient `NavigationManager` to derive the base URI for relative asset URLs.
- Uses an injected `ICacheStorageService` to read/write cached payloads keyed on the crafted `HttpRequestMessage`.
- Falls back to `HttpClient.SendAsync` and returns `string.Empty` when the download is not successful, letting callers decide how to handle missing resources.

Because this service is registered through `AddHttpClient<IStaticAssetService, StaticAssetService>()`, you can layer Polly handlers or resilience policies at the host level without modifying the implementation.

---

## 6. Antiforgery-aware HttpClient factory

`IAntiforgeryHttpClientFactory` ([src/ISynergy.Framework.UI.Blazor/Abstractions/Security/IAntiforgeryHttpClientFactory.cs](src/ISynergy.Framework.UI.Blazor/Abstractions/Security/IAntiforgeryHttpClientFactory.cs)) is realized by [src/ISynergy.Framework.UI.Blazor/Security/AntiforgeryHttpClientFactory.cs](src/ISynergy.Framework.UI.Blazor/Security/AntiforgeryHttpClientFactory.cs):

```csharp
public async Task<HttpClient> CreateClientAsync(string clientName = "authorizedClient")
{
    var token = await _jSRuntime.InvokeAsync<string>("getAntiForgeryToken");

    var client = _httpClientFactory.CreateClient(clientName);
    client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token);

    return client;
}
```

- Relies on a JavaScript function (`getAntiForgeryToken`) to pull the server-issued token before stamping the outgoing request header.
- Registered as a transient dependency so each call can fetch the most recent token and client configuration.

---

## 7. Minimal JS interop helpers

[src/ISynergy.Framework.UI.Blazor/Extensions/JsRuntimeExtensions.cs](src/ISynergy.Framework.UI.Blazor/Extensions/JsRuntimeExtensions.cs) currently adds `FocusElementById`, implemented by invoking `document.getElementById` and calling `focus()` on the returned element reference. Extend this module when more DOM utilities are needed instead of sprinkling raw `IJSRuntime` calls through components.

---

## 8. Static typing for caching and navigation menus

All service abstractions live under [src/ISynergy.Framework.UI.Blazor/Abstractions/Services](src/ISynergy.Framework.UI.Blazor/Abstractions/Services). Besides form-factor and static assets, it contains `ICacheStorageService` and `INavigationMenuService`, giving consumers a stable contract regardless of platform. When implementing custom behavior, keep the namespace alignment (`ISynergy.Framework.UI.Abstractions.Services`) so `ConfigureServices` will pick up replacements automatically through DI.

---

## 9. Usage checklist

- Always call `ConfigureServices<...>` from your host builder to ensure `ILanguageService`, `IMessengerService`, antiforgery, and static asset plumbing are registered once.
- Inherit UI components from `View<TViewModel>` whenever you need automatic initialization, property change refreshes, and command subscription.
- Use `CommandBinding`/`CommandClick` helpers instead of manual lambda wiring so the disabled state and `CanExecute` remain synchronized.
- Fetch server-hosted JSON or text through `IStaticAssetService` instead of creating ad-hoc `HttpClient` instances; caching is automatic and mockable.
- Request `IAntiforgeryHttpClientFactory` whenever you need to post to protected endpoints from client code.

---

## 10. Troubleshooting tips

| Symptom | Likely cause | Remedy |
| --- | --- | --- |
| ViewModel logic never runs | `View<TViewModel>` not registered as scoped or not injected | Add the component through DI (lifetime `Scoped`) and ensure the ViewModel itself is registered. |
| Buttons stay disabled | Command not exposing `CanExecuteChanged` or component not using `CommandBinding` | Use `CommandBinding` so the helper subscribes to events discovered via reflection. |
| Static assets return empty string | Remote fetch failed and service returned `string.Empty` | Inspect server logs/responses; the service only suppresses errors by returning empty payloads. |
| Antiforgery header missing | JS function `getAntiForgeryToken` absent on page | Ensure `_Host.cshtml` or your layout defines the script before calling `IAntiforgeryHttpClientFactory`. |

Pair this skill with **ui-core** for shared abstractions and **ui-maui** when hosting Razor content inside `BlazorWebView`.
