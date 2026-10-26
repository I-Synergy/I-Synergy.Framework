namespace Sample.Blazor.UI.Tests.Infrastructure;

/// <summary>
/// The DOM contract of the Blazor sample app, as verified against the rendered page.
/// <para>
/// The shell is hand-rolled (see <c>samples/Sample.Blazor/Components/Layout/MainLayout.razor</c> and
/// <c>wwwroot/site.css</c>): <c>FluentLayout</c> / <c>FluentLayoutItem</c> / <c>FluentNav</c> are no
/// longer used, because their chrome does not match the native (MAUI / WinUI) sample shells. The
/// layout is now plain, stable markup:
/// </para>
/// <list type="bullet">
///   <item><description><c>div.app-shell</c> — flex root (100dvh) carrying <c>data-app-theme</c>.</description></item>
///   <item><description><c>aside.app-sidebar</c> — permanent 300px column at &gt;=901px, off-canvas
///   (fixed, 280px, <c>translateX(-100%)</c>) drawer at &lt;=900px, slid in by the <c>is-open</c> class.</description></item>
///   <item><description><c>header.app-header</c> — mobile nav toggle, breadcrumb, header actions.</description></item>
///   <item><description><c>main.app-content</c> — wraps <c>article#article</c>.</description></item>
///   <item><description><c>div.app-nav-scrim</c> — rendered only while the drawer is open.</description></item>
/// </list>
/// <para>
/// What is still Fluent UI: the provider container and the provider/decorator components below, plus the
/// real web components (spinner, overlay, dialog, text input). Those keep their <c>fluent-*</c> class or
/// tag. The distinction is itself the regression guard: a silently degraded (unresolved) component tag
/// and a plain class-carrying div are different things, and only one of them is correct per component.
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

    // ---- shell root ---------------------------------------------------------
    /// <summary>Flex root of the hand-rolled shell; carries <c>data-app-theme</c> ("dark" / "light").</summary>
    public const string LayoutRoot = ".app-shell";

    // ---- shell areas --------------------------------------------------------
    /// <summary>The sidebar column / off-canvas drawer (also the navigation's container).</summary>
    public const string AreaSidebar = "aside.app-sidebar";

    /// <summary>The sidebar while the mobile drawer is slid in.</summary>
    public const string AreaSidebarOpen = "aside.app-sidebar.is-open";

    public const string AreaHeader = "header.app-header";
    public const string AreaContent = "main.app-content";

    /// <summary>Bottom block of the sidebar: the Help link and the version label.</summary>
    public const string AreaFooter = ".app-sidebar-footer";

    public const string Brand = ".app-brand";
    public const string Breadcrumb = ".app-breadcrumb";

    /// <summary>Theme + Settings icon buttons; the (mobile only) nav toggle is <see cref="NavToggle"/>.</summary>
    public const string HeaderActions = ".app-header-actions";
    public const string IconButton = "button.app-icon-button";

    /// <summary>
    /// The mobile hamburger in the header. <c>display:none</c> at &gt;=901px, <c>inline-flex</c> at
    /// &lt;=900px — desktop has no collapsing toggle at all, the sidebar is always there.
    /// </summary>
    public const string NavToggle = "button.app-nav-toggle";

    /// <summary>Semi transparent backdrop; only rendered while the drawer is open, hidden at &gt;900px.</summary>
    public const string NavScrim = "div.app-nav-scrim";

    /// <summary>The sidebar's footer link shares the nav item styling; assertions scope through <see cref="NavRoot"/>.</summary>
    public const string SidebarFooterNavItem = ".app-sidebar-footer a.app-nav-item";

    // ---- providers (class based, rendered by FluentProviders) ---------------
    public const string Providers = ".fluent-providers";
    public const string DialogProvider = ".fluent-dialog-provider";
    public const string ToastProvider = ".fluent-toast-provider";
    public const string TooltipProvider = ".fluent-tooltip-provider";

    // ---- navigation (plain markup rendered by MainLayout) ------------------
    public const string NavRoot = "nav.app-nav";
    public const string NavMenuId = "#main_menu";

    /// <summary>
    /// The four route links. Scoped to <see cref="NavRoot"/> on purpose: the sidebar footer's Help link
    /// carries the same <c>app-nav-item</c> class but is not a navigation item.
    /// </summary>
    public const string NavItem = "nav.app-nav a.app-nav-item";

    /// <summary>The label span of a nav item; the leading icon span is <c>aria-hidden</c> decoration.</summary>
    public const string NavItemLabel = "nav.app-nav a.app-nav-item > span:not(.app-nav-icon)";

    /// <summary><c>NavLink</c> adds <c>active</c> (the CSS hook) plus <c>aria-current="page"</c>.</summary>
    public const string NavItemActive = "nav.app-nav a.app-nav-item.active";

    public const string NavItemActiveAttribute = "aria-current";

    /// <summary>v4 markup: <c>FluentNavLink</c> emitted <c>fluent-nav-link</c>. It must not exist in v5.</summary>
    public const string RemovedNavLinkClass = "a.fluent-nav-link";

    /// <summary>v4 markup: <c>FluentNavMenu</c> emitted a <c>fluent-nav-menu</c> element. It must not exist in v5.</summary>
    public const string RemovedNavMenuTag = "fluent-nav-menu";

    /// <summary>The v4/v5 <c>FluentLayout</c> column; the hand-rolled shell must not render it.</summary>
    public const string RemovedLayoutItem = ".fluent-layout-item";

    /// <summary>The v4/v5 <c>FluentNav</c> root; the hand-rolled shell must not render it.</summary>
    public const string RemovedNavRoot = "nav.fluent-nav";

    // ---- web components (real custom elements) ------------------------------
    public const string Spinner = "fluent-spinner";

    /// <summary>v4 name of <see cref="Spinner"/>; renamed in v5.</summary>
    public const string RemovedProgressRing = "fluent-progress-ring";

    public const string Overlay = "fluent-overlay";
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
