# I-Synergy Framework Globalization

Comprehensive localization and globalization services for multi-language .NET applications with live language switching support.

## Live Localization with LocalizedStrings

The `LocalizedStrings` singleton provides automatic UI refresh when the application language changes at runtime.

### Using LocalizedStrings with Binding

`LocalizedStrings` exposes an indexer that resolves localized strings via `ILanguageService`:

```csharp
// Get a localized string
string greeting = LocalizedStrings.Default["Greeting"];  // Returns localized value
```

### XAML Binding (WPF, WinUI, MAUI)

Bind UI directly to the indexer for live updates:

```xml
<!-- WPF -->
<TextBlock Text="{Binding [WelcomeMessage], Source={x:Static local:LocalizedStrings.Default}}" />

<!-- WinUI (note: x:Static not supported, bind to resource instead) -->
<!-- See UI.WinUI readme for workaround -->

<!-- MAUI (use m:LocalizedStrings markup extension for trim-friendly compiled bindings) -->
<Label Text="{m:LocalizedStrings WelcomeMessage}" />
```

### Refresh and Change Notifications

When the application language changes, call `ApplyCulture()` to automatically refresh all bound UI:

```csharp
// Change the application culture
var newCulture = new CultureInfo("de-DE");
newCulture.ApplyCulture();  // Automatically calls LocalizedStrings.Default.Refresh()

// All UI bindings to LocalizedStrings indexer update immediately
```

The refresh is triggered via:
- `PropertyChanged` event with "Item[]" property name (WPF/WinUI)
- `Changed` event (Blazor and other frameworks)

### ILanguageService Unchanged

`ILanguageService` and `LanguageService` remain unmodified — they only retrieve strings. The `LocalizedStrings` class is additive and does not break existing code.