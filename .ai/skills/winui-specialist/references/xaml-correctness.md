# WinUI 3 UI Design and XAML Correctness

Fluent Design layout choices, theming, typography, spacing, data binding, attached properties, layout anti-patterns, and accessibility. Load this when writing or reviewing XAML.
---


## UI Design and XAML Correctness

### App Type → Anchor Control

| App Type | Anchor Control | Reference App |
|----------|---------------|---------------|
| Settings / config tool | `NavigationView` Left + `SettingsCard` | Windows Settings |
| Document / session editor | `TabView` + full-width content | Windows Terminal |
| Hierarchical browser | `TreeView` + `ListView` + `BreadcrumbBar` | File Explorer |
| Developer tool / dashboard | `NavigationView` + card layout | Dev Home |
| Single-purpose utility | Mode switcher + compact grid | Calculator |

### Navigation
- 2–7 sections → `NavigationView`
- Document tabs → `TabView`
- 2–3 modes → `SelectorBar`
- Breadcrumb trail → `BreadcrumbBar`

### Data Display
- Vertical list → `ListView`
- Grid/tiles → `GridView` or `ItemsRepeater` + `UniformGridLayout`
- Hierarchy → `TreeView`
- Master-detail → `ListView` + detail `Grid`

### Input
- Text → `TextBox` | Number → `NumberBox` | Search → `AutoSuggestBox`
- Boolean → `ToggleSwitch` | One-of-2/3 → `RadioButtons` | One-of-4+ → `ComboBox`

### Feedback
- Blocking decision → `ContentDialog`
- Contextual action → `Flyout` / `MenuFlyout`
- Inline status → `InfoBar`

### Theming Rules

```xml
<!-- CORRECT: StaticResource redirect in theme dictionary -->
<StaticResource x:Key="MyBrush" ResourceKey="ControlFillColorDefaultBrush" />

<!-- WRONG: inline SolidColorBrush allocates new object per theme -->
<SolidColorBrush x:Key="MyBrush" Color="{StaticResource ControlFillColorDefault}" />
```

- `{ThemeResource BrushName}` at usage sites — updates on theme change
- `ResourceKey` must end in `Brush` — target the `SolidColorBrush`, not the `Color`
- Always define all three variants: `Light`, `Dark`, `HighContrast` — never `Default`
- No hardcoded hex colors or `Color="Blue"` anywhere in production XAML

### High Contrast

Only 8 system brushes in HC dictionaries: `SystemColorWindowColorBrush`, `SystemColorWindowTextColorBrush`, `SystemColorHighlightColorBrush`, `SystemColorHighlightTextColorBrush`, `SystemColorButtonFaceColorBrush`, `SystemColorButtonTextColorBrush`, `SystemColorHotlightColorBrush`, `SystemColorGrayTextColorBrush`.

Set `HighContrastAdjustment = None` at app level.

### Typography Styles (use styles, never raw FontSize)

| Style | Size | Weight | Use For |
|-------|------|--------|---------|
| `CaptionTextBlockStyle` | 12px | Regular | Labels, timestamps |
| `BodyTextBlockStyle` | 14px | Regular | Body (default — don't set explicitly) |
| `BodyStrongTextBlockStyle` | 14px | Semibold | Emphasized body |
| `SubtitleTextBlockStyle` | 20px | Semibold | Section headers |
| `TitleTextBlockStyle` | 28px | Semibold | Page titles |
| `TitleLargeTextBlockStyle` | 40px | Semibold | Large feature titles |
| `DisplayTextBlockStyle` | 68px | Semibold | Hero text |

Use `SemiBold`, never `Bold`. Minimum 12px.

### Spacing Grid

Margins, padding, and sizes must be multiples of 4: **4, 8, 12, 16, 24, 32, 48**.

- `ControlCornerRadius` (4px) for controls — never hardcode
- `OverlayCornerRadius` (8px) for overlays — never hardcode
- `RowSpacing`/`ColumnSpacing` instead of spacer elements
- No negative margins

### Data Binding

```xml
<!-- CORRECT: TwoWay with PropertyChanged so UIA set-value commits immediately -->
<TextBox Text="{x:Bind ViewModel.Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
```

- `{x:Bind}` over `{Binding}`, always explicit `Mode=OneWay`/`TwoWay`
- `x:DataType` on every `DataTemplate`
- Commands over Click/Tapped handlers (MVVM)
- `VisualStateManager` for visual property changes, not code-behind
- No `IValueConverter` — prefer `x:Bind` with static functions

**Bool negation and Visibility helpers** (define as static in code-behind):
```csharp
public static Visibility BoolToVisibility(bool v) => v ? Visibility.Visible : Visibility.Collapsed;
public static Visibility InvertBoolToVisibility(bool v) => v ? Visibility.Collapsed : Visibility.Visible;
public static bool IsNotBusy(bool isLoading) => !isLoading;
```
```xml
Visibility="{x:Bind local:MainPage.BoolToVisibility(ViewModel.IsLoading), Mode=OneWay}"
```
❌ NEVER use `Converter={x:Null}` — crashes at runtime.

### Attached Properties in Code-Behind

```csharp
// ❌ WRONG — object initializer doesn't work for attached properties
var btn = new Button { AutomationProperties = { AutomationId = "BtnSave" } };

// ✅ CORRECT
var btn = new Button { Content = "Save" };
AutomationProperties.SetAutomationId(btn, "BtnSave");
AutomationProperties.SetName(btn, "Save button");
Grid.SetRow(btn, 1);
```

### Layout Anti-Patterns

| ❌ Don't | ✅ Do Instead |
|----------|--------------|
| Centered floating card on background | Content fills window with padding |
| Custom pill/segment tab switcher | `NavigationView` Top or `SelectorBar` |
| Equal-width 50/50 split | Fixed sidebar (300–360px) + flexible main |
| Hardcoded colors (`#FF0000`) | `{ThemeResource}` brushes |
| `ScrollViewer` around `ListView` | ListView has built-in scrolling |

### Accessibility (required on every control)

```csharp
using Microsoft.UI.Xaml.Automation;
AutomationProperties.SetAutomationId(btn, "BtnSave");
AutomationProperties.SetName(btn, "Save document");
```

- `AutomationProperties.AutomationId` on **every** interactive control
- `AutomationProperties.Name` on icon-only controls
- Semantic elements (`Button`, `HyperlinkButton`) — never clickable `Border`/`TextBlock`
- No information conveyed by color alone
