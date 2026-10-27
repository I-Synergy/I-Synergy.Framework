# WPF → WinUI 3 Migration

Namespace/control maps, dispatcher threading, the critical migration rules, and the audit command. Load this when porting a WPF app to WinUI 3.
---


## WPF → WinUI 3 Migration

### Namespace Map

| WPF | WinUI 3 |
|-----|---------|
| `System.Windows` | `Microsoft.UI.Xaml` |
| `System.Windows.Controls` | `Microsoft.UI.Xaml.Controls` |
| `System.Windows.Media` | `Microsoft.UI.Xaml.Media` |
| `System.Windows.Input` | `Microsoft.UI.Xaml.Input` |
| `System.Windows.Data` | `Microsoft.UI.Xaml.Data` |
| `System.Windows.Threading.Dispatcher` | `Microsoft.UI.Dispatching.DispatcherQueue` |
| `PresentationCore` / `PresentationFramework` | Remove entirely |

### Control Map

| WPF | WinUI 3 |
|-----|---------|
| `DataGrid` | `ListView` with Grid column headers |
| `WrapPanel` | `ItemsRepeater` + `UniformGridLayout` |
| `TabControl` | `TabView` |
| `Menu` / `MenuItem` | `MenuBar` / `MenuFlyoutItem` |
| `ToolBar` | `CommandBar` |

### Threading

```csharp
// WPF
Application.Current.Dispatcher.Invoke(() => { /* UI work */ });

// WinUI 3
DispatcherQueue.GetForCurrentThread().TryEnqueue(() => { /* UI work */ });
```

### Critical Migration Rules

- ❌ NEVER reference `PresentationCore`, `PresentationFramework`, or `System.Windows.Controls`
- ❌ NEVER add `<UseWPF>true</UseWPF>` — silently corrupts the build
- ❌ NEVER overwrite `App.xaml` / `App.xaml.cs` — merge WPF code into the WinUI 3 boilerplate
- ✅ Remove ALL `System.Windows.Media.Imaging` references at migration start
- ✅ Replace with `Microsoft.UI.Xaml.Media.Imaging.BitmapImage`
- ✅ Replace custom MVVM → CommunityToolkit.Mvvm (`[ObservableProperty]`, `[RelayCommand]`)
- ✅ Replace `.resx` → `.resw` in `Strings\en-us\`
- ✅ Replace `DynamicResource` → `{ThemeResource}`

### Audit WPF Usage

```powershell
Select-String -Path (Get-ChildItem -Recurse -Filter "*.cs" |
    Where-Object { $_.FullName -notlike "*\obj\*" }) -Pattern "System\.Windows\."
```
