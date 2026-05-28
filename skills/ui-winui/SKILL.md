```skill
{
  "name": "i-synergy-ui-winui",
  "description": "WinUI 3 implementation details: Fluent Design shell, ContentDialog service, camera/file integrations, title bar customization, Windows 11 accent sync.",
  "version": "2026.02.10",
  "sources": [
    "src/ISynergy.Framework.UI.WinUI",
    "samples/Sample.WinUI",
    "docs/ThemeWindow-Selection-Fix.md"
  ],
  "license": "See license.md in repository root"
}
```

# I-Synergy Framework UI WinUI Skill (SKILL.md)

Build Windows 10/11 apps with the official WinUI 3 shell from [src/ISynergy.Framework.UI.WinUI/readme.md](src/ISynergy.Framework.UI.WinUI/readme.md) and [samples/Sample.WinUI](samples/Sample.WinUI).

---

## 1. Hosting Model

```csharp
public sealed partial class App : Application
{
    private IHost? _host;
    private Window? _window;

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
                        svc.AddSingleton<ICameraService, CameraService>();
                        svc.AddSingleton<IUpdateService, UpdateService>();
                    },
                    typeof(App).Assembly);

                services.RegisterAssemblies(typeof(App).Assembly, asm => asm.Name!.StartsWith("MyApp"));
                services.AddTransient<MainWindow>();
            })
            .Build();

        await _host.StartAsync();
        _host.Services.GetRequiredService<IThemeService>().ApplyTheme();

        _window = _host.Services.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}
```

- `ICameraService` is unique to WinUI; it wraps `Windows.Media.Capture` APIs for photo capture + file picking.
- `IThemeService.ApplyTheme()` syncs with system accent colors.

---

## 2. Fluent Shell Layout

```xml
<Window ...>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <Grid Grid.Row="0" Background="{ThemeResource SystemControlAcrylicElementBrush}">
            <CommandBar>
                <AppBarButton Icon="Add" Label="New" Command="{Binding NewCommand}"/>
                <AppBarButton Icon="OpenFile" Label="Open" Command="{Binding OpenCommand}"/>
                <AppBarSeparator/>
                <AppBarButton Icon="Setting" Label="Settings" Command="{Binding SettingsCommand}"/>
            </CommandBar>
        </Grid>

        <Frame Grid.Row="1" x:Name="MainFrame"/>

        <Grid Grid.Row="2" Background="{ThemeResource SystemControlAcrylicElementBrush}">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>

            <TextBlock Text="{Binding StatusMessage}" Margin="12,8"/>
            <ProgressRing Grid.Column="1" Width="20" Height="20" Margin="12,8" IsActive="{Binding BusyService.IsBusy}"/>
        </Grid>
    </Grid>
</Window>
```

- Use Acrylic/Mica brushes to follow Fluent Design guidelines.
- Bind `ProgressRing.IsActive` to `BusyService.IsBusy` for global progress.

---

## 3. Dialog + Navigation

```csharp
public sealed class ShellViewModel : ViewModel
{
    public AsyncRelayCommand<Type> NavigateCommand { get; }

    public ShellViewModel(ICommonServices commonServices, INavigationService navigationService, ILogger<ShellViewModel> logger)
        : base(commonServices, logger)
    {
        NavigateCommand = new AsyncRelayCommand<Type>(type => navigationService.NavigateAsync(type));
    }

    private Task EditSettingsAsync() =>
        CommonServices.DialogService.ShowDialogAsync<SettingsDialog, SettingsViewModel, Settings>();
}
```

- WinUI dialog service maps to `ContentDialog`/`MessageDialog`.
- Navigation service wraps `Frame.Navigate` and tracks `CanGoBack`.

---

## 4. Camera Integration

```csharp
public sealed class ProfileViewModel : ViewModel
{
    private readonly ICameraService _camera;

    public BitmapImage? ProfileImage
    {
        get => GetValue<BitmapImage?>();
        set => SetValue(value);
    }

    public AsyncRelayCommand TakePhotoCommand { get; }
    public AsyncRelayCommand PickPhotoCommand { get; }

    public ProfileViewModel(ICommonServices commonServices, ICameraService camera, ILogger<ProfileViewModel> logger)
        : base(commonServices, logger)
    {
        _camera = camera;
        TakePhotoCommand = new AsyncRelayCommand(TakePhotoAsync);
        PickPhotoCommand = new AsyncRelayCommand(PickPhotoAsync);
    }

    private async Task TakePhotoAsync()
    {
        var photo = await _camera.TakePhotoAsync();
        if (photo is null) return;
        ProfileImage = await photo.ToBitmapImageAsync();
    }
}
```

Helper extension:

```csharp
public static class FileResultExtensions
{
    public static async Task<BitmapImage> ToBitmapImageAsync(this FileResult file)
    {
        var bitmap = new BitmapImage();
        using var stream = await file.OpenReadAsync();
        await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
        return bitmap;
    }
}
```

---

## 5. Title Bar Customization

```csharp
public sealed partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            var titleBar = AppWindow.TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
        }

        DataContext = viewModel;
    }
}
```

- Follow Windows 11 guidelines: transparent background, custom hover colors, support dark/light mode transitions via `IThemeService`.

---

## 6. Custom Controls

| Control | Purpose | Notes |
| --- | --- | --- |
| `BladeView` | Multi-pane detail layout | Bind `ItemsSource` and `SelectedItem` to show stacked blades. |
| `ImageBrowser` | Asset gallery with add/remove actions | Integrates with `IFileService` for local storage. |
| `SplashScreen` | Modern splash with progress indicator | Replace default WinUI splash; link to telemetry for load times. |

```xml
<controls:BladeView ItemsSource="{x:Bind ViewModel.Blades}" SelectedItem="{x:Bind ViewModel.SelectedBlade, Mode=TwoWay}" />
```

---

## 7. Troubleshooting

| Issue | Cause | Fix |
| --- | --- | --- |
| Window opens without content | `MainWindow` not resolved from DI | Always request `MainWindow` via `_host.Services.GetRequiredService<MainWindow>()`. |
| Navigation fails with `NullReferenceException` | `Frame` not assigned | Assign `NavigationService.Frame = MainFrame` inside the window constructor. |
| Camera service throws capability error | Missing `webcam`/`microphone` capabilities | Add `webcam` and `microphone` to Package.appxmanifest. |
| Title bar buttons revert to default colors | `AppWindowTitleBar` not supported (older OS) | Guard with `AppWindowTitleBar.IsCustomizationSupported()`. |

---

## 8. Pairing with Other Skills

- Combine with **UI Core** for authentication/token/theme logic.
- Use **Blazor UI** or **MAUI UI** skills when embedding Razor components (WinUI desktop shell shares the same ViewModels).
- Reference [docs/ThemeWindow-Selection-Fix.md](docs/ThemeWindow-Selection-Fix.md) for consistent theme picker UI between WinUI and MAUI.

Deliver WinUI apps that feel natively Fluent while sharing the same MVVM infrastructure, dialogs, and services as every other I-Synergy shell.
