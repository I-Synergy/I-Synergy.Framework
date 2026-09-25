namespace Sample.Blazor.UI.Tests.Infrastructure;

/// <summary>
/// The DOM contract of the Fluent UI Blazor v5 sample app, as verified against the rendered page.
/// <para>
/// IMPORTANT: v5 renders the layout / navigation / provider components as plain elements carrying a
/// <c>fluent-*</c> CSS class (a <c>&lt;div class="fluent-layout"&gt;</c>), NOT as <c>fluent-layout</c>
/// custom elements. Only the components that wrap a real web component (spinner, overlay, button,
/// text input, drawer, ...) appear as <c>fluent-*</c> tags. Both kinds are asserted below, and the
/// distinction is itself the regression guard: a silently degraded (unresolved) component tag and a
/// plain class-carrying div are different things, and only one of them is correct per component.
/// </para>
/// </summary>
internal static class SampleDom
{
    // ---- routes -------------------------------------------------------------
    public const string HomeRoute = "/";
    public const string CounterRoute = "/counter";
    public const string WeatherRoute = "/weather";
    public const string CommandDemoRoute = "/commanddemoview";

    public static readonly string[] AllRoutes = [HomeRoute, CounterRoute, WeatherRoute, CommandDemoRoute];

    // ---- layout (class based, rendered by FluentLayout / FluentLayoutItem) ---
    public const string LayoutRoot = ".fluent-layout";
    public const string LayoutItem = ".fluent-layout-item";
    public const string LayoutHamburger = ".fluent-layout-hamburger";

    public static string LayoutArea(string area) => $".fluent-layout-item[area=\"{area}\"]";

    public const string AreaHeader = ".fluent-layout-item[area=\"header\"]";
    public const string AreaNavigation = ".fluent-layout-item[area=\"nav\"]";
    public const string AreaContent = ".fluent-layout-item[area=\"content\"]";
    public const string AreaFooter = ".fluent-layout-item[area=\"footer\"]";

    // ---- providers (class based, rendered by FluentProviders) ---------------
    public const string Providers = ".fluent-providers";
    public const string DialogProvider = ".fluent-dialog-provider";
    public const string ToastProvider = ".fluent-toast-provider";
    public const string TooltipProvider = ".fluent-tooltip-provider";

    // ---- navigation (class based, rendered by FluentNav / FluentNavItem) ----
    public const string NavRoot = "nav.fluent-nav";
    public const string NavMenuId = "#main_menu";
    public const string NavItem = "a.fluent-navitem";
    public const string NavItemActive = "a.fluent-navitem.active";

    /// <summary>v4 markup: <c>FluentNavLink</c> emitted <c>fluent-nav-link</c>. It must not exist in v5.</summary>
    public const string RemovedNavLinkClass = "a.fluent-nav-link";

    /// <summary>v4 markup: <c>FluentNavMenu</c> emitted a <c>fluent-nav-menu</c> element. It must not exist in v5.</summary>
    public const string RemovedNavMenuTag = "fluent-nav-menu";

    // ---- web components (real custom elements) ------------------------------
    public const string Spinner = "fluent-spinner";

    /// <summary>v4 name of <see cref="Spinner"/>; renamed in v5.</summary>
    public const string RemovedProgressRing = "fluent-progress-ring";

    public const string Overlay = "fluent-overlay";
    public const string Drawer = "fluent-drawer";
    public const string TextInput = "fluent-text-input";

    /// <summary>v4 name of <see cref="TextInput"/>; removed in v5 (rendered as inert HTML).</summary>
    public const string RemovedTextField = "fluent-text-field";

    public const string Dialog = "fluent-dialog";

    /// <summary>The custom title template slot rendered by <c>BaseModelDialog</c>.</summary>
    public const string DialogTitleSlot = "fluent-dialog [slot='title']";

    /// <summary>The custom footer/action template slot (Cancel/Add) rendered by <c>BaseModelDialog</c>.</summary>
    public const string DialogActionSlot = "fluent-dialog [slot='action']";

    /// <summary>v4 dialog chrome, removed in v5.</summary>
    public const string RemovedDialogHeader = "fluent-dialog-header";

    // ---- command demo page --------------------------------------------------
    /// <summary>The command demo counter readout; used as the circuit-liveness probe (see CircuitLivenessTests).</summary>
    public const string CommandDemoCounter = "p:has-text('Current count:')";

    /// <summary>The page content container rendered by the layout; pages' own markup lives inside it.</summary>
    public const string ContentArticle = "#article";

    // ---- misc ---------------------------------------------------------------
    public const string BusyIndicator = "#busyIndicator";
    public const string BlazorErrorUi = "#blazor-error-ui";
    public const string MessageBarProvider = ".fluent-message-bar-provider";
}
