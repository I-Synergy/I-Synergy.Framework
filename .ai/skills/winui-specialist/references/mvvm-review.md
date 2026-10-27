# WinUI 3 MVVM Code Review Checklist

The review checklist to run before declaring a WinUI 3 change done: MVVM compliance, `x:Bind` correctness, performance, security, globalization.
---


## MVVM Code Review Checklist

### MVVM Compliance
- [ ] ViewModels extend `ObservableObject`, use `[ObservableProperty]` partial properties (not fields)
- [ ] Commands use `[RelayCommand]` — no manual `ICommand` implementations
- [ ] No UI types in ViewModels (`SolidColorBrush`, `Visibility`, `BitmapImage`)
- [ ] No business logic in code-behind — only navigation, dialog coordination, event wiring
- [ ] `async Task` for async methods, `async void` only for event handlers
- [ ] Never replace `ObservableCollection<T>` — use `.Clear()` + re-add

### x:Bind and Data Binding
- [ ] All bindings use `{x:Bind}`, not `{Binding}`
- [ ] `Mode=OneWay` or `TwoWay` set explicitly — `OneTime` default causes blank UI
- [ ] `x:DataType` on every `DataTemplate`
- [ ] No nested nullable paths without `FallbackValue`
- [ ] Command bindings can use `OneTime` (commands don't change)

### Performance
- [ ] Long lists use `ListView`/`GridView` (virtualized), not `StackPanel` + `foreach`
- [ ] `x:Load` for content not always visible
- [ ] Heavy work off UI thread via `Task.Run` or `async/await`
- [ ] No `.Result` / `.Wait()` / `.GetAwaiter().GetResult()` — deadlocks UI thread
- [ ] `using` on all disposable objects

### Security
- [ ] No secrets or API keys in source code
- [ ] No `Process.Start` with unsanitized user input
- [ ] File paths from user input validated before `File.Delete` / `File.WriteAllText`

### Globalization
- [ ] User-facing strings use `x:Uid` (XAML) / `ResourceLoader` (C#)
- [ ] Resources in `Strings/en-us/Resources.resw`
- [ ] Date/number formatting uses `CultureInfo.CurrentCulture`
- [ ] No string concatenation for user-facing messages
