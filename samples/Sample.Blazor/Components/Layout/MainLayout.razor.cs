using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;

using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Messages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace Sample.Components.Layout;

public partial class MainLayout : IDisposable
{
    /// <summary>
    /// Location of the JS workaround for the upstream Fluent UI Blazor v5.0.0 mobile
    /// breakpoint race. Safe to delete together with <see cref="ApplyLayoutBreakpointFixAsync"/>.
    /// </summary>
    private const string LayoutBreakpointFixModulePath = "./js/layout-breakpoint-fix.js";

    // Add cancellation token source and disposal tracking
    private readonly CancellationTokenSource _cts = new();
    private bool _disposed;

    private ILocalSettings? _localSettings;
    private string? _version;

    private readonly NavigationManager _navigationManager;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly ICommonServices _commonServices;
    private readonly ISettingsService _settingsService;
    private readonly IContext _context;
    private readonly RequestCancellationService _requestCancellationService;
    private readonly IJSRuntime _jsRuntime;

    public MainLayout(
        ICommonServices commonServices,
        IDialogService dialogService,
        INotificationService notificationService,
        NavigationManager navigationManager,
        RequestCancellationService requestCancellationService,
        IContext context,
        ISettingsService settingsService,
        IJSRuntime jsRuntime)
    {
        _commonServices = commonServices;
        _context = context;
        _settingsService = settingsService;

        _dialogService = dialogService;
        _notificationService = notificationService;
        _navigationManager = navigationManager;
        _requestCancellationService = requestCancellationService;
        _jsRuntime = jsRuntime;
    }

    [Parameter]
    public RenderFragment? Body { get; set; }

    private string ContentClass => _localSettings?.ShowConsole == true && _navigationManager!.Uri != _navigationManager!.BaseUri
       ? "content with-console"
       : "content full-width";

    private string ArticleClass => _localSettings?.ShowConsole == true && _navigationManager!.Uri != _navigationManager!.BaseUri
        ? "with-console"
        : "full-width";

    protected override void OnInitialized()
    {
        _version = _commonServices.InfoService.ProductVersion.ToString();

        _commonServices.BusyService.PropertyChanged += BusyService_PropertyChanged;

        // v5: ShowInfoAsync/ShowWarningAsync/ShowErrorAsync already return the dialog result
        // (Task<DialogResult>); there is no intermediate reference object and DialogResult has
        // no .Result property, so the v4 'await dialogResult.Result' unwrapping is gone.
        _commonServices.MessengerService.Register<ShowInformationMessage>(this, async m => // NOSONAR - async void event handler pattern required by messenger service
        {
            _ = await _dialogService.ShowInfoAsync(m.Content.Message, m.Content.Title, _commonServices.LanguageService.GetString("OK"));
        });

        _commonServices.MessengerService.Register<ShowWarningMessage>(this, async m => // NOSONAR - async void event handler pattern required by messenger service
        {
            _ = await _dialogService.ShowWarningAsync(m.Content.Message, m.Content.Title, _commonServices.LanguageService.GetString("OK"));
        });

        _commonServices.MessengerService.Register<ShowErrorMessage>(this, async m => // NOSONAR - async void event handler pattern required by messenger service
        {
            _ = await _dialogService.ShowErrorAsync(m.Content.Message, m.Content.Title, _commonServices.LanguageService.GetString("OK"));
        });

        _navigationManager.LocationChanged += NavigationManager_LocationChanged;

        if (_settingsService.LocalSettings is ILocalSettings localSettings)
        {
            _localSettings = localSettings;
            _localSettings.PropertyChanged += LocalSettings_PropertyChanged;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // MainLayout is the Router's layout: it is not re-created on a client side navigation
            // (only its Body parameter changes), so firstRender happens exactly once per page load.
            await ApplyLayoutBreakpointFixAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// WORKAROUND for an upstream Fluent UI Blazor v5.0.0 bug — safe to delete, together with
    /// <c>wwwroot/js/layout-breakpoint-fix.js</c> and <see cref="LayoutBreakpointFixModulePath"/>.
    /// <para>
    /// <c>FluentLayout</c> keeps the mobile/desktop decision in .NET (<c>IsMobile</c>), but the only
    /// thing that ever sets it is its own JS callback, and on a page that is *loaded* below the
    /// mobile breakpoint that callback is never delivered: the layout's <c>ResizeObserver</c> runs
    /// its one initial pass from <c>Startup.afterStarted()</c> — before the interactive
    /// <c>DotNetObjectReference</c> exists — and the later <c>Initialize(helper, id)</c> call only
    /// attaches the helper without re-creating the observer. The rendered DOM is then in mobile mode
    /// while .NET still reports desktop, so the navigation stays in the CSS-hidden
    /// <c>[area="nav"]</c> column and the hamburger drawer is rendered empty: a mobile user cannot
    /// navigate. The module re-supplies that one missing notification once the circuit is live.
    /// </para>
    /// <para>
    /// <c>NavigationDeferredLoading="true"</c> (see MainLayout.razor) is unrelated to this race and
    /// must stay: it is what keeps exactly one copy of the navigation — and of <c>id="main_menu"</c> —
    /// in the DOM, because the hamburger drawer reuses the navigation area's content.
    /// </para>
    /// </summary>
    private async Task ApplyLayoutBreakpointFixAsync()
    {
        try
        {
            var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", LayoutBreakpointFixModulePath);

            try
            {
                await module.InvokeVoidAsync("applyInitialBreakpointFix");
            }
            finally
            {
                // The module stays cached in the browser, so the .NET handle is not needed
                // afterwards and is disposed rather than held for the whole circuit.
                await module.DisposeAsync();
            }
        }
        catch (JSException)
        {
            // Best effort: a workaround for an upstream bug must not take the app down.
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

    public void ToggleConsole()
    {
        if (_localSettings is not null)
        {
            _settingsService.LocalSettings.ShowConsole = !_localSettings.ShowConsole;
            _settingsService.SaveLocalSettings();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Clears the registered message bars on navigation.
    /// v5 removed the <c>FluentMessageBarProvider.ClearAfterNavigation</c> parameter and has no
    /// replacement: the closest equivalent is to close the message bars through the notification
    /// service. Unlike the v4 flag (which only affected one provider/section), this closes every
    /// message bar currently registered — acceptable here because the sample only hosts the
    /// MESSAGES_TOP provider.
    /// The layout-level hamburger owns its own open/close state in v5, so the v4
    /// '#navmenu-toggle' / mobile-menu bookkeeping is no longer needed here.
    /// </summary>
    private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (!_disposed)
        {
            _ = _notificationService.CloseAllMessageBarsAsync();
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
        _version = null;
    }
}
