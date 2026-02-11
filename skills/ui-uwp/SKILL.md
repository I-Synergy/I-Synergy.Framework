```skill
{
  "name": "i-synergy-ui-uwp",
  "description": "UWP implementation of the ISynergy UI stack (Frame navigation, ContentDialog service, adaptive layouts, WinUI 2 controls).",
  "version": "2026.02.10",
  "sources": [
    "src/ISynergy.Framework.UI.UWP",
    "samples/Sample.UWP",
    "docs/ThemeWindow-Synchronization.md"
  ],
  "license": "See license.md in repository root"
}
```

# I-Synergy Framework UI UWP Skill (SKILL.md)

Follow this guide when targeting Windows 10 devices with the UWP shell described in [src/ISynergy.Framework.UI.UWP/readme.md](src/ISynergy.Framework.UI.UWP/readme.md).

> Minimum OS: **10.0.19041.0** (Version 2004). For older builds stay on package v1.x.

---

## 1. Hosting + Shell Setup

```csharp
sealed partial class App : Application
{
    private IHost? _host;

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var infoService = new InfoService();
                infoService.LoadAssembly(typeof(App).Assembly);

                services.ConfigureServices<Context, CommonServices, ExceptionHandlerService, SettingsService, Properties.Resources>(
                    context.Configuration,
                    infoService,
                    svc =>
                    {
                        svc.AddSingleton<IDialogService, DialogService>();
                        svc.AddSingleton<INavigationService, NavigationService>();
                        svc.AddSingleton<IThemeService, ThemeService>();
                        svc.AddSingleton<IFileService<FileResult>, FileService>();
                        svc.AddSingleton<IClipboardService, ClipboardService>();
                    },
                    typeof(App).Assembly);

                services.RegisterAssemblies(typeof(App).Assembly, asm => asm.Name!.StartsWith("MyApp"));
            })
            .Build();

        await _host.StartAsync();
        _host.Services.GetRequiredService<IThemeService>().ApplyTheme();

        var rootFrame = Window.Current.Content as Frame ?? new Frame();
        Window.Current.Content = rootFrame;

        if (rootFrame.Content is null)
        {
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
        }

        Window.Current.Activate();
    }
}
```

- Assign the global `Frame` to `INavigationService` (usually in `MainPage` constructor).
- Use `Application.OnSuspending`/`OnResuming` to persist `ISettingsService` data if needed.

---

## 2. XAML Patterns

```xml
<Page ... xmlns:muxc="using:Microsoft.UI.Xaml.Controls">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <CommandBar Grid.Row="0">
            <AppBarButton Icon="Add" Label="New" Command="{x:Bind ViewModel.NewCommand}"/>
            <AppBarButton Icon="Save" Label="Save" Command="{x:Bind ViewModel.SaveCommand}"/>
            <AppBarSeparator/>
            <AppBarButton Icon="Setting" Label="Settings" Command="{x:Bind ViewModel.SettingsCommand}"/>
        </CommandBar>

        <Frame Grid.Row="1" x:Name="ContentFrame"/>

        <muxc:ProgressBar Grid.Row="2" IsIndeterminate="{x:Bind ViewModel.BusyService.IsBusy, Mode=OneWay}"/>
    </Grid>
</Page>
```

- Prefer `x:Bind` for compiled bindings to keep UWP performance high.
- Use WinUI 2 controls (`Microsoft.UI.Xaml.Controls`) such as `NavigationView`, `ProgressBar`, `InfoBar` for modern visuals.

---

## 3. Dialog + Navigation

```csharp
public sealed class ProductViewModel : ViewModel
{
    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    public ProductViewModel(ICommonServices commonServices, IProductService svc, ILogger<ProductViewModel> logger)
        : base(commonServices, logger)
    {
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
    }

    private async Task SaveAsync()
    {
        await _svc.SaveAsync(Product);
        await CommonServices.DialogService.ShowInformationAsync("Saved", "Success");
    }

    private async Task DeleteAsync()
    {
        var result = await CommonServices.DialogService.ShowMessageAsync(
            "Delete this product?",
            "Confirm",
            MessageBoxButtons.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            await _svc.DeleteAsync(Product.Id);
            await CommonServices.NavigationService.GoBackAsync();
        }
    }
}
```

- Dialog service internally calls `ContentDialog`/`MessageDialog`; keep logic inside ViewModels for reuse in WinUI/MAUI/Blazor.

---

## 4. Adaptive UI Checklist

| Scenario | Recommendation |
| --- | --- |
| Handle screen sizes | Use `VisualStateManager` with `AdaptiveTrigger MinWindowWidth` thresholds (e.g., 640, 1024). |
| Orientation changes | Bind to `DisplayInformation.AutoRotationPreferences` if necessary. |
| Xbox/TV layout | Increase padding/margins and use `Gamepad` friendly focus states. |
| Input methods | Ensure `FocusVisualKind` covers mouse, touch, gamepad. |

Example Visual State snippet:

```xml
<VisualStateManager.VisualStateGroups>
    <VisualStateGroup>
        <VisualState x:Name="NarrowState">
            <VisualState.StateTriggers>
                <AdaptiveTrigger MinWindowWidth="0" />
            </VisualState.StateTriggers>
            <VisualState.Setters>
                <Setter Target="Sidebar.Visibility" Value="Collapsed" />
            </VisualState.Setters>
        </VisualState>
        <VisualState x:Name="WideState">
            <VisualState.StateTriggers>
                <AdaptiveTrigger MinWindowWidth="1024" />
            </VisualState.StateTriggers>
            <VisualState.Setters>
                <Setter Target="Sidebar.Visibility" Value="Visible" />
            </VisualState.Setters>
        </VisualState>
    </VisualStateGroup>
</VisualStateManager.VisualStateGroups>
```

---

## 5. Platform Requirements + Manifest

`Package.appxmanifest` snippet:

```xml
<TargetDeviceFamily Name="Windows.Universal" MinVersion="10.0.19041.0" MaxVersionTested="10.0.26100.0" />
<Capabilities>
    <rescap:Capability Name="runFullTrust"/>
    <DeviceCapability Name="webcam"/>
    <DeviceCapability Name="microphone"/>
</Capabilities>
```

- Add `broadFileSystemAccess`, `internetClient`, etc., as needed by your ViewModels/services.
- If you still target older OS versions, pin `I-Synergy.Framework.UI.UWP` to v1.x.

---

## 6. Performance Tips

- Use `x:Bind` (Mode=OneWay/TwoWay) for compiled bindings.
- Enable UI virtualization (`ItemsStackPanel` with `AreStickyGroupHeadersEnabled`).
- Break large lists into incremental-loading data sources.
- Avoid synchronous file I/O; always use async APIs.

---

## 7. Troubleshooting

| Issue | Cause | Fix |
| --- | --- | --- |
| Build fails with target version error | Target version < 10.0.19041.0 | Update `TargetDeviceFamily MinVersion`. |
| `ContentDialog` not showing | Missing `await` or called before window activation | Ensure `await DialogService.Show...` and call after `Window.Current.Activate()`. |
| File picker throws | Capability not declared | Add `broadFileSystemAccess` or specific device capability. |
| Navigation state lost after suspend | Not persisting Frame state | Handle `OnSuspending` and call `Frame.GetNavigationState()` + `SetNavigationState()` on resume. |

---

## 8. Skill Pairings

- Use **UI Core** for authentication/token/theme logic.
- Use **UI WinUI** when migrating to WinUI 3 or sharing ViewModels between UWP and WinUI.
- Use **Blazor/MAUI** skills when embedding Razor components via WebView or reusing the same ViewModels cross-shell.

This UWP skill keeps legacy Windows 10 deployments aligned with the rest of the I-Synergy UI ecosystem while reusing the same MVVM infrastructure.
