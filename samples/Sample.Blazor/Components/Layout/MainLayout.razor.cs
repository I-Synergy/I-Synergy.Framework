using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Messages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace Sample.Components.Layout;

public partial class MainLayout
{
    // Add cancellation token source and disposal tracking
    private readonly CancellationTokenSource _cts = new();
    private bool _disposed;

    private const string JAVASCRIPT_FILE = "./_content/I-Synergy.Framework.UI.Blazor/js/layout.js";
    private ILocalSettings? _localSettings;
    private string? _version;
    private bool _mobile;
    private string? _prevUri;
    private bool _menuChecked = true;

    private readonly NavigationManager _navigationManager;
    private readonly IDialogService _dialogService;
    private readonly IJSRuntime _JSRuntime;
    private readonly ICommonServices _commonServices;
    private readonly ISettingsService _settingsService;
    private readonly IContext _context;
    private readonly RequestCancellationService _requestCancellationService;

    public MainLayout(
        ICommonServices commonServices,
        IDialogService dialogService,
        NavigationManager navigationManager,
        IJSRuntime jSRuntime,
        RequestCancellationService requestCancellationService)
    {
        _commonServices = commonServices;

        _context = _commonServices.ScopedContextService.GetRequiredService<IContext>();
        _settingsService = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>();

        _dialogService = dialogService;
        _navigationManager = navigationManager;
        _JSRuntime = jSRuntime;
        _requestCancellationService = requestCancellationService;
    }

    [Parameter]
    public RenderFragment? Body { get; set; }

    private string ContentClass => _localSettings.ShowConsole && _navigationManager.Uri != _navigationManager.BaseUri
       ? "content with-console"
       : "content full-width";

    private string ArticleClass => _localSettings.ShowConsole && _navigationManager.Uri != _navigationManager.BaseUri
        ? "with-console"
        : "full-width";

    protected override void OnInitialized()
    {
        _version = _commonServices.InfoService.ProductVersion.ToString();
        _prevUri = _navigationManager.Uri;

        _commonServices.BusyService.PropertyChanged += BusyService_PropertyChanged;

        MessengerService.Default.Register<ShowInformationMessage>(this, async m =>
        {
            var dialogResult = await _dialogService.ShowInfoAsync(m.Content.Message, m.Content.Title, LanguageService.Default.GetString("OK"));

            if (dialogResult is not null)
            {
                var result = await dialogResult.Result;

                //if (result.Cancelled)
                //    return MessageBoxResult.Cancel;

                //return MessageBoxResult.OK;
            }

            //return MessageBoxResult.None;
        });

        MessengerService.Default.Register<ShowWarningMessage>(this, async m =>
        {
            var dialogResult = await _dialogService.ShowWarningAsync(m.Content.Message, m.Content.Title, LanguageService.Default.GetString("OK"));

            if (dialogResult is not null)
            {
                var result = await dialogResult.Result;

                //if (result.Cancelled)
                //    return MessageBoxResult.Cancel;

                //return MessageBoxResult.OK;
            }

            //return MessageBoxResult.None;
        });

        MessengerService.Default.Register<ShowErrorMessage>(this, async m =>
        {
            var dialogResult = await _dialogService.ShowErrorAsync(m.Content.Message, m.Content.Title, LanguageService.Default.GetString("OK"));

            if (dialogResult is not null)
            {
                var result = await dialogResult.Result;

                //if (result.Cancelled)
                //    return MessageBoxResult.Cancel;

                //return MessageBoxResult.OK;
            }

            //return MessageBoxResult.None;
        });

        _navigationManager.LocationChanged += NavigationManager_LocationChanged;

        if (_settingsService.LocalSettings is ILocalSettings localSettings)
        {
            _localSettings = localSettings;
            _localSettings.PropertyChanged += LocalSettings_PropertyChanged;
        }
    }

    private void BusyService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        StateHasChanged();
    }

    private void LocalSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Check for disposal before proceeding
        if (_disposed)
            return;

        if (firstRender)
        {
            try
            {
                // Don't pass the cancellation token to JSRuntime.InvokeAsync
                var jsModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", JAVASCRIPT_FILE);

                if (_disposed)
                {
                    await jsModule.DisposeAsync();
                    return;
                }

                // Don't pass the cancellation token here either
                _mobile = await jsModule.InvokeAsync<bool>("isDevice");
                await jsModule.DisposeAsync();
            }
            catch (Exception ex) when (!_disposed)
            {
                // Log exception
                Console.Error.WriteLine($"Error in OnAfterRenderAsync: {ex.Message}");
            }
        }
    }

    public void ToggleConsole()
    {
        if (_localSettings is not null)
        {
            _settingsService.LocalSettings.ShowConsole = !_localSettings.ShowConsole;
            _settingsService.SaveLocalSettingsAsync().GetAwaiter().GetResult();
            StateHasChanged();
        }
    }

    private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (!e.IsNavigationIntercepted && new Uri(_prevUri!).AbsolutePath != new Uri(e.Location).AbsolutePath)
        {
            _prevUri = e.Location;
            if (_mobile && _menuChecked == true)
            {
                _menuChecked = false;
                StateHasChanged();
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        // Cancel any pending operations
        try
        {
            _cts.Cancel();
            _cts.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Already disposed, ignore
        }

        // Unregister message handlers
        _commonServices.MessengerService.Unregister<AuthenticationChangedMessage>(this);
        _commonServices.MessengerService.Unregister<ShowInformationMessage>(this);
        _commonServices.MessengerService.Unregister<ShowWarningMessage>(this);
        _commonServices.MessengerService.Unregister<ShowErrorMessage>(this);

        // Unsubscribe from events
        _commonServices.BusyService.PropertyChanged -= BusyService_PropertyChanged;
        _navigationManager.LocationChanged -= NavigationManager_LocationChanged;

        if (_localSettings is not null)
        {
            _localSettings.PropertyChanged -= LocalSettings_PropertyChanged;
            _localSettings = null;
        }

        // Clear references to help GC
        _prevUri = null;
        _version = null;
    }
}
