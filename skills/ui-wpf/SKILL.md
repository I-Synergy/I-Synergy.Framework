```skill
{
  "name": "i-synergy-ui-wpf",
  "description": "Windows Presentation Foundation (WPF) implementation of the ISynergy UI stack: dialog/navigation services, theme system, BladeView controls, update pipeline.",
  "version": "2026.02.10",
  "sources": [
    "src/ISynergy.Framework.UI.WPF",
    "samples/Sample.WPF",
    "docs/ThemeWindow-Selection-Enhancement.md"
  ],
  "license": "See license.md in repository root"
}
```

# I-Synergy Framework UI WPF Skill (SKILL.md)

The following sections are derived from the concrete classes in [src/ISynergy.Framework.UI.Wpf](src/ISynergy.Framework.UI.Wpf). They describe how the library actually configures services, views, navigation, and dialogs.

---

## 1. Hosting & DI (`WpfAppBuilderExtensions.ConfigureServices`)

[src/ISynergy.Framework.UI.Wpf/Extensions/WPFAppBuilderExtensions.cs](src/ISynergy.Framework.UI.Wpf/Extensions/WPFAppBuilderExtensions.cs) defines `ConfigureServices<TContext, ...>` which Host builders should call:

```csharp
services.AddOptions();
services.Configure<ClientApplicationOptions>(configuration.GetSection(nameof(ClientApplicationOptions)).BindWithReload);

services.TryAddSingleton<IInfoService>(s => infoService);
services.TryAddSingleton<ILanguageService>(s => languageService);
services.TryAddSingleton<IMessengerService, MessengerService>();

services.TryAddScoped<TContext>();
services.TryAddScoped<IContext>(s => s.GetRequiredService<TContext>());
services.TryAddSingleton<TCommonServices>();
services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<TCommonServices>());
services.TryAddSingleton<IExceptionHandlerService, TExceptionHandlerService>();
services.TryAddScoped<ISettingsService, TSettingsService>();
services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();

services.TryAddSingleton<IScopedContextService, ScopedContextService>();
services.TryAddSingleton<IBusyService, BusyService>();
services.TryAddSingleton<IClipboardService, ClipboardService>();
services.TryAddSingleton<INavigationService, NavigationService>();
services.TryAddSingleton<IDialogService, DialogService>();
services.TryAddSingleton<IFileService<FileResult>, FileService>();
```

- `RegisterAssemblies(assembly, assemblyFilter)` auto-registers every View and ViewModel; no manual `AddTransient<MainViewModel>` calls required.
- `AddUpdatesIntegration()` in the same file wires `UpdateOptions` + `IUpdateService` if you need the desktop updater.

---

## 2. View base class

[src/ISynergy.Framework.UI.Wpf/Controls/View.cs](src/ISynergy.Framework.UI.Wpf/Controls/View.cs) is the page base for every WPF view:

- Exposes a strongly-typed `IViewModel ViewModel` property; assigning it updates `DataContext` and binds `Page.Title` to the ViewModel’s `Title` property via `Binding`.
- Implements `IDisposable` and disposes the ViewModel when the Page is unloaded, ensuring scoped services/releases happen consistently.
- Use it by inheriting `public partial class OrdersView : View` and resolving it through DI.

---

## 3. Navigation service behavior

`INavigationService` is implemented in [src/ISynergy.Framework.UI.Wpf/Services/NavigationService.cs](src/ISynergy.Framework.UI.Wpf/Services/NavigationService.cs):

- Maintains its own `_backStack` of `IViewModel` instances (capped at 10) so `GoBackAsync()` reuses initialized ViewModels and triggers `OnBackStackChanged` only when necessary.
- Uses `NavigationExtensions.CreatePage<TViewModel>` and `IScopedContextService` to resolve views, wait for their `Loaded` events, and call `InitializeAsync()` before showing them.
- Supports blade panes (`OpenBladeAsync`, `RemoveBlade`) and prevents multiple instances of the same blade type from being added to an owner.
- Cancels running commands when swapping blades by reflecting over `IAsyncRelayCommand` properties and calling `CancelAllCommands()` when needed.

---

## 4. Dialog service internals

`DialogService` ([src/ISynergy.Framework.UI.Wpf/Services/DialogService.cs](src/ISynergy.Framework.UI.Wpf/Services/DialogService.cs)) provides two categories of dialogs:

- Message boxes: `ShowMessageAsync` maps `MessageBoxButtons` to `System.Windows.MessageBoxButton` and translates the result back to `ISynergy.Framework.Mvvm.Enumerations.MessageBoxResult`. Localization (`ILanguageService`) supplies default titles (`TitleError`, `TitleInfo`, etc.).
- Window-based dialogs: `ShowDialogAsync<TWindow, TViewModel, TEntity>` resolves both the view (`TWindow : IWindow`) and ViewModel from `IScopedContextService`, wires `Closed` handlers to dispose correctly, calls `InitializeAsync()`, then awaits `window.ShowAsync<TEntity>()`.

If ViewModel initialization fails the service logs via `ILogger<DialogService>` and unsubscribes the handler before rethrowing.

---

## 5. Clipboard/File/Update helpers

- `IClipboardService`, `IFileService<FileResult>`, and `IUpdateService` registrations happen inside `ConfigureServices`. Their implementations live under [src/ISynergy.Framework.UI.Wpf/Services](src/ISynergy.Framework.UI.Wpf/Services).
- `FileService` wraps OS dialogs; `UpdateService` consumes `UpdateOptions` to check/install packages. Because they are singletons, you can inject them directly into ViewModels (`DocumentViewModel`, etc.) without additional plumbing.

---

## 6. Resource dictionary helpers

`AddToResourceDictionary<T>` in the extension file swaps ResourceDictionary entries by name, retrieving instances from `IScopedContextService`. Use it inside `App.xaml.cs` to expose services (e.g., `INavigationService`) to XAML.

---

## 7. Troubleshooting mapped to code

| Symptom | Where it originates | How to fix |
| --- | --- | --- |
| Navigation hangs | `NavigationService.GetNavigationBladeAsync` awaiting `Loaded` never completes when views are never inserted | Ensure resolved `View` is inserted into a Frame/Page before calling `NavigateAsync`, or shorten the timeout if you manipulate visual tree manually. |
| Dialog never closes | `DialogService.CreateDialogAsync` only closes when `IViewModelDialog<TEntity>.Closed` fires | Always raise `Closed` from your dialog ViewModel once work finishes. |
| Back stack grows indefinitely | `_backStack` only clears when `CleanBackStackAsync` is called | Invoke `CleanBackStackAsync()` for workflows with deep navigation, especially after logout. |
| ViewModel disposed too early | `View.Dispose` disposes ViewModel on GC | Avoid sharing a single ViewModel instance across multiple Views; resolve a new View for each navigation.

---

Pair this skill with **ui-core** for shared authentication/context services and with **ui-winui**/**ui-uwp** when you port the same ViewModels to other desktop shells.
