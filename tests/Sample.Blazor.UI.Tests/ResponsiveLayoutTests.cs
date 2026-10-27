using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// The responsive shell contract.
/// <para>
/// Re-baselined after the shell rewrite. The shell used to be <c>FluentLayout</c>, which moved the
/// navigation into a <c>fluent-drawer</c> below its 768px breakpoint and exposed the desktop/mobile
/// decision through a <c>mobile</c> attribute set from JS (the source of an upstream race these tests
/// originally diagnosed). The hand-rolled shell instead keeps <em>one</em> navigation in the DOM at all
/// times and changes only its presentation with pure CSS:
/// </para>
/// <list type="bullet">
///   <item><description>&gt;=901px: <c>aside.app-sidebar</c> is a permanent 300px column (position
///   static) and <c>button.app-nav-toggle</c> is <c>display:none</c> — desktop has no collapse toggle.</description></item>
///   <item><description>&lt;=900px: the same sidebar becomes a fixed 280px off-canvas drawer
///   (<c>translateX(-100%)</c>) that the header toggle slides in with the <c>is-open</c> class, with
///   <c>div.app-nav-scrim</c> rendered behind it while it is open.</description></item>
/// </list>
/// <para>
/// Each test therefore asserts the behaviour the new shell actually has: at 900px and below the
/// navigation must be reachable (toggle -&gt; drawer -&gt; link), and above it must be permanently
/// present with no toggle. Nothing is asserted about the removed Fluent drawer.
/// </para>
/// </summary>
[TestClass]
public class ResponsiveLayoutTests : SamplePageTest
{
    private const int MobileWidth = 480;
    private const int MobileHeight = 800;

    [TestMethod]
    public async Task Mobile_At480px_TheToggleOpensTheOffCanvasSidebarWithAScrim()
    {
        await OpenMobileHomeAsync();

        var beforeClick = await DescribeResponsiveStateAsync();
        Log($"mobile @480px before toggle click: {beforeClick}");
        var before = System.Text.Json.JsonDocument.Parse(beforeClick).RootElement;

        // The navigation exists exactly once and is off-canvas until the toggle is used: a translated
        // element still has a box, so "off-canvas" is asserted on the geometry (its right edge is at or
        // left of the viewport's left edge), not on Playwright's visibility.
        Assert.AreEqual(1, before.GetProperty("mainMenuIds").GetInt32(),
            $"id=\"main_menu\" must appear exactly once. Observed DOM: {beforeClick}");
        Assert.AreEqual(4, before.GetProperty("navItemCount").GetInt32(),
            $"The navigation must hold the 4 route items. Observed DOM: {beforeClick}");
        Assert.IsFalse(before.GetProperty("sidebarOpen").GetBoolean(),
            $"The drawer must start closed. Observed DOM: {beforeClick}");
        Assert.IsTrue(before.GetProperty("sidebarBox").GetProperty("right").GetInt32() <= 0,
            $"The sidebar must start off-canvas. Observed DOM: {beforeClick}");
        Assert.IsFalse(before.GetProperty("scrimPresent").GetBoolean(),
            $"No scrim may be rendered while the drawer is closed. Observed DOM: {beforeClick}");

        var toggle = NavToggle();
        await Expect(toggle).ToBeVisibleAsync();
        await Expect(toggle).ToHaveAttributeAsync("aria-expanded", "false");
        await Expect(toggle).ToHaveAttributeAsync("aria-controls", "main_menu");

        await toggle.ClickAsync();
        await WaitForDrawerOpenAsync();

        var afterClick = await DescribeResponsiveStateAsync();
        Log($"mobile @480px after toggle click: {afterClick}");
        var after = System.Text.Json.JsonDocument.Parse(afterClick).RootElement;

        Assert.IsTrue(after.GetProperty("sidebarOpen").GetBoolean(),
            $"The drawer did not get the is-open class. Observed DOM: {afterClick}");
        Assert.IsTrue(after.GetProperty("sidebarBox").GetProperty("x").GetInt32() >= 0,
            $"The opened drawer must be on screen. Observed DOM: {afterClick}");
        Assert.IsTrue(after.GetProperty("scrimPresent").GetBoolean(),
            $"The scrim must be rendered while the drawer is open. Observed DOM: {afterClick}");
        Assert.AreEqual("true", after.GetProperty("toggleExpanded").GetString(),
            $"The toggle must report aria-expanded=true while the drawer is open. Observed DOM: {afterClick}");
        Assert.AreEqual(4, after.GetProperty("onscreenNavItems").GetInt32(),
            $"All 4 navigation items must be on screen while the drawer is open, otherwise a mobile user " +
            $"cannot navigate. Observed DOM: {afterClick}");
        Assert.AreEqual(1, await CountAsync(SampleDom.NavMenuId),
            "The navigation must exist exactly once while the drawer is open.");

        // Clicking the scrim (away from the 280px drawer itself) closes the drawer again.
        var scrim = Page.Locator(SampleDom.NavScrim);
        await scrim.ClickAsync(new LocatorClickOptions { Position = new Position { X = MobileWidth - 20, Y = MobileHeight / 2 } });
        await WaitForDrawerClosedAsync();

        var afterScrimClick = await DescribeResponsiveStateAsync();
        Log($"mobile @480px after scrim click: {afterScrimClick}");
        var closed = System.Text.Json.JsonDocument.Parse(afterScrimClick).RootElement;
        Assert.IsFalse(closed.GetProperty("sidebarOpen").GetBoolean(),
            $"Clicking the scrim must close the drawer. Observed DOM: {afterScrimClick}");
        Assert.IsFalse(closed.GetProperty("scrimPresent").GetBoolean(),
            $"The scrim must be removed once the drawer is closed. Observed DOM: {afterScrimClick}");
        Assert.IsTrue(closed.GetProperty("sidebarBox").GetProperty("right").GetInt32() <= 0,
            $"The closed drawer must be off-canvas again. Observed DOM: {afterScrimClick}");

        await AssertBaselineAsync("mobile-drawer");
    }

    /// <summary>
    /// Repurposed. The original test was a root-cause probe for the <c>FluentLayout</c> JS/`.NET`
    /// breakpoint race: it loaded at desktop width first, crossed the breakpoint at runtime and checked
    /// whether the navigation relocated into the hamburger drawer. That premise no longer exists — the
    /// hand-rolled shell decides desktop/mobile purely in CSS (<c>@media (max-width: 900px)</c>), so
    /// nothing can race. The test now keeps its *runtime* breakpoint-transition shape, which is still
    /// worth guarding (a media query is easy to get wrong on resize, and the prerender/hydration seam is
    /// where a stale desktop layout would show up): loading at desktop width must give the permanent
    /// sidebar and no toggle, and resizing to 480px at runtime must switch both.
    /// </summary>
    [TestMethod]
    public async Task Mobile_CrossingTheBreakpointAtRuntime_SwitchesToTheOffCanvasDrawer()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await WaitForShellReadyAsync();

        var desktop = System.Text.Json.JsonDocument.Parse(await DescribeResponsiveStateAsync()).RootElement;
        Log($"diagnosis: loaded at desktop width: {await DescribeResponsiveStateAsync()}");
        Assert.AreEqual("static", desktop.GetProperty("sidebarPosition").GetString(),
            "At desktop width the sidebar must be a permanent (static) column.");
        Assert.AreEqual("none", desktop.GetProperty("toggleDisplay").GetString(),
            "At desktop width the toggle must be hidden.");

        await Page.SetViewportSizeAsync(MobileWidth, MobileHeight);
        // Wait for the CSS breakpoint to be applied *and* for the drawer's transform transition to
        // settle: the sidebar switches to position:fixed immediately, but it slides off-canvas over
        // 0.2s, so measuring before the transition ends would report the desktop position.
        await Page.WaitForFunctionAsync(
            """
            () => {
                const sidebar = document.querySelector('aside.app-sidebar');
                if (!sidebar) return false;
                return getComputedStyle(sidebar).position === 'fixed'
                    && sidebar.getBoundingClientRect().right <= 0;
            }
            """,
            null,
            new() { Timeout = 15_000 });

        var mobile = System.Text.Json.JsonDocument.Parse(await DescribeResponsiveStateAsync()).RootElement;
        var mobileJson = await DescribeResponsiveStateAsync();
        Log($"diagnosis: after crossing the breakpoint at runtime: {mobileJson}");

        Assert.AreEqual("fixed", mobile.GetProperty("sidebarPosition").GetString(),
            $"The sidebar must become an off-canvas drawer after the runtime breakpoint change. Observed DOM: {mobileJson}");
        Assert.IsTrue(mobile.GetProperty("sidebarBox").GetProperty("right").GetInt32() <= 0,
            $"The drawer must be off-canvas after the runtime breakpoint change. Observed DOM: {mobileJson}");

        await Expect(NavToggle()).ToBeVisibleAsync();

        // The drawer must still work after the transition, not just on a fresh mobile load.
        await NavToggle().ClickAsync();
        await WaitForDrawerOpenAsync();
        var opened = System.Text.Json.JsonDocument.Parse(await DescribeResponsiveStateAsync()).RootElement;
        Assert.AreEqual(4, opened.GetProperty("onscreenNavItems").GetInt32(),
            "The drawer must expose the 4 navigation items after the runtime breakpoint change.");

        await AssertBaselineAsync("mobile-diagnosis");
    }

    [TestMethod]
    public async Task Mobile_At480px_TheUserCanNavigateFromTheDrawer()
    {
        await OpenMobileHomeAsync();

        await NavToggle().ClickAsync();
        await WaitForDrawerOpenAsync();

        var drawer = Page.Locator(SampleDom.AreaSidebar);
        var counterLink = drawer.Locator(SampleDom.NavItem).Filter(new LocatorFilterOptions { HasTextString = "Counter" });

        await Expect(counterLink).ToBeVisibleAsync();
        await counterLink.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/counter$"));
        await Expect(Page.Locator("h1")).ToHaveTextAsync("Counter");

        // The drawer must not survive the navigation (MainLayout is reused by client-side navigation,
        // so an unhandled drawer would stay open over the destination page).
        await WaitForDrawerClosedAsync();
        await Expect(Page.Locator(SampleDom.NavScrim)).ToHaveCountAsync(0);

        Log($"mobile navigation from the drawer reached {Page.Url}");
        await AssertBaselineAsync("mobile-navigation");
    }

    /// <summary>
    /// Repurposed. The original test asserted a *desktop* hamburger that collapsed and restored the
    /// navigation column — a <c>FluentLayout</c> feature the new design deliberately does not have: the
    /// sidebar is permanent above 900px and the toggle does not exist there. Rather than delete it, it
    /// now asserts exactly that: on desktop there is no visible toggle, the sidebar is permanently
    /// present (also across a client-side navigation, where a collapsible column would lose its state)
    /// and no drawer scrim is ever rendered.
    /// </summary>
    [TestMethod]
    public async Task Desktop_HasNoCollapsingToggle_TheSidebarIsPermanent()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await WaitForShellReadyAsync();

        var sidebar = Page.Locator(SampleDom.AreaSidebar);
        await Expect(sidebar).ToBeVisibleAsync();
        Assert.IsFalse(await sidebar.GetAttributeAsync("class") is { } cls && cls.Contains("is-open"),
            "The sidebar must never carry the mobile drawer's is-open class on desktop.");

        var toggle = NavToggle();
        Assert.AreEqual(1, await toggle.CountAsync(), "exactly one nav toggle must exist in the header.");
        await Expect(toggle).ToBeHiddenAsync();
        await Expect(Page.Locator(SampleDom.NavScrim)).ToHaveCountAsync(0);

        var desktop = System.Text.Json.JsonDocument.Parse(await DescribeResponsiveStateAsync()).RootElement;
        Log($"desktop state: {await DescribeResponsiveStateAsync()}");
        Assert.AreEqual("static", desktop.GetProperty("sidebarPosition").GetString(),
            "The desktop sidebar must be a permanent (static) column, not a fixed drawer.");
        Assert.AreEqual(300, desktop.GetProperty("sidebarBox").GetProperty("w").GetInt32(),
            "The desktop sidebar must be 300px wide.");
        Assert.AreEqual(4, desktop.GetProperty("onscreenNavItems").GetInt32(),
            "All 4 navigation items must be on screen on desktop.");

        // Client-side navigation must leave the permanent column in place.
        await Page.Locator(SampleDom.NavItem).Filter(new LocatorFilterOptions { HasTextString = "Weather" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/weather$"));

        await Expect(sidebar).ToBeVisibleAsync();
        await Expect(toggle).ToBeHiddenAsync();
        await Expect(Page.Locator(SampleDom.NavScrim)).ToHaveCountAsync(0);
        var afterNavigation = System.Text.Json.JsonDocument.Parse(await DescribeResponsiveStateAsync()).RootElement;
        Assert.AreEqual("static", afterNavigation.GetProperty("sidebarPosition").GetString(),
            "The sidebar must still be the permanent desktop column after navigating.");

        await AssertBaselineAsync("desktop-sidebar");
    }

    /// <summary>
    /// Measurement, not a pass/fail judgement: reports whether the sidebar column shows a background band
    /// below the (content height) navigation, as reported by a separate agent. Ported to the new shell:
    /// the column is <c>aside.app-sidebar</c> and the navigation is <c>nav.app-nav</c>.
    /// </summary>
    [TestMethod]
    public async Task Desktop_Sidebar_ReportsColumnAndNavigationHeightsAndBackgrounds()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await WaitForShellReadyAsync();

        var measurements = await Page.EvaluateAsync<string>(
            """
            () => {
                const column = document.querySelector('aside.app-sidebar');
                const nav = column?.querySelector('nav.app-nav');
                const article = document.getElementById('article');
                const shell = document.querySelector('.app-shell');
                const describe = (el) => {
                    if (!el) return null;
                    const s = getComputedStyle(el);
                    const r = el.getBoundingClientRect();
                    return {
                        height: Math.round(r.height),
                        width: Math.round(r.width),
                        background: s.backgroundColor,
                        padding: `${s.paddingTop} ${s.paddingBottom}`,
                        overflowY: s.overflowY
                    };
                };
                const c = describe(column), n = describe(nav);
                return JSON.stringify({
                    column: c,
                    nav: n,
                    article: describe(article),
                    shell: describe(shell),
                    seamPx: (c && n) ? Math.round(c.height - n.height) : null,
                    sameBackground: (c && n) ? c.background === n.background : null
                });
            }
            """);

        Log($"sidebar column measurements: {measurements}");

        var state = System.Text.Json.JsonDocument.Parse(measurements).RootElement;
        Assert.AreNotEqual(System.Text.Json.JsonValueKind.Null, state.GetProperty("column").ValueKind, "The sidebar column must exist.");
        Assert.AreNotEqual(System.Text.Json.JsonValueKind.Null, state.GetProperty("nav").ValueKind, "The navigation must exist.");

        var columnHeight = state.GetProperty("column").GetProperty("height").GetInt32();
        var navHeight = state.GetProperty("nav").GetProperty("height").GetInt32();
        var seam = state.GetProperty("seamPx").GetInt32();

        Assert.IsTrue(columnHeight > 0, "The sidebar column has no height.");
        Assert.IsTrue(navHeight > 0, "The navigation has no height.");
        Assert.IsTrue(columnHeight >= navHeight, "The navigation cannot be taller than its sidebar column.");

        Console.WriteLine(
            $"[evidence] two-tone check: sidebar column is {columnHeight}px tall, nav.app-nav is {navHeight}px tall, " +
            $"difference (the brand/footer block around the nav) = {seam}px. " +
            $"column background={state.GetProperty("column").GetProperty("background").GetString()}, " +
            $"nav background={state.GetProperty("nav").GetProperty("background").GetString()}.");
    }

    /// <summary>Loads the home page at phone width; the shell has no JS breakpoint state to wait for.</summary>
    private async Task OpenMobileHomeAsync()
    {
        await Page.SetViewportSizeAsync(MobileWidth, MobileHeight);
        await NavigateAsync(SampleDom.HomeRoute);
        await WaitForShellReadyAsync();
    }

    private ILocator NavToggle() => Page.Locator(SampleDom.NavToggle);

    private Task WaitForDrawerOpenAsync() =>
        Page.WaitForFunctionAsync(
            """
            () => {
                const sidebar = document.querySelector('aside.app-sidebar');
                return !!sidebar
                    && sidebar.classList.contains('is-open')
                    && !!document.querySelector('div.app-nav-scrim')
                    && sidebar.getBoundingClientRect().x >= 0;
            }
            """,
            null,
            new() { Timeout = 15_000 });

    /// <summary>
    /// Waits until the drawer is closed: the class is gone, the scrim is unrendered and the CSS
    /// transition (0.2s <c>transform</c>) has finished, i.e. the drawer is fully off-canvas again.
    /// </summary>
    private Task WaitForDrawerClosedAsync() =>
        Page.WaitForFunctionAsync(
            """
            () => {
                const sidebar = document.querySelector('aside.app-sidebar');
                return !!sidebar
                    && !sidebar.classList.contains('is-open')
                    && !document.querySelector('div.app-nav-scrim')
                    && sidebar.getBoundingClientRect().right <= 0;
            }
            """,
            null,
            new() { Timeout = 15_000 });

    /// <summary>
    /// Records the responsive shell state. Everything a test asserts on is measured from the live DOM:
    /// the drawer's geometry (an off-canvas drawer still has a box), the toggle's computed display, and
    /// whether the navigation items are actually inside the viewport.
    /// </summary>
    private async Task<string> DescribeResponsiveStateAsync()
    {
        return await Page.EvaluateAsync<string>(
            """
            () => {
                const shell = document.querySelector('.app-shell');
                const sidebar = document.querySelector('aside.app-sidebar');
                const toggle = document.querySelector('button.app-nav-toggle');
                const scrim = document.querySelector('div.app-nav-scrim');
                const items = [...document.querySelectorAll('nav.app-nav a.app-nav-item')];
                const box = (el) => {
                    const r = el.getBoundingClientRect();
                    return { x: Math.round(r.x), right: Math.round(r.right), w: Math.round(r.width), h: Math.round(r.height) };
                };
                return JSON.stringify({
                    viewport: { w: innerWidth, h: innerHeight },
                    shellPresent: !!shell,
                    sidebarPresent: !!sidebar,
                    sidebarBox: sidebar ? box(sidebar) : null,
                    sidebarPosition: sidebar ? getComputedStyle(sidebar).position : null,
                    sidebarTransform: sidebar ? getComputedStyle(sidebar).transform : null,
                    sidebarOpen: sidebar ? sidebar.classList.contains('is-open') : null,
                    togglePresent: !!toggle,
                    toggleDisplay: toggle ? getComputedStyle(toggle).display : null,
                    toggleExpanded: toggle?.getAttribute('aria-expanded') ?? null,
                    toggleControls: toggle?.getAttribute('aria-controls') ?? null,
                    scrimPresent: !!scrim,
                    navItemCount: items.length,
                    onscreenNavItems: items.filter(e => {
                        const r = e.getBoundingClientRect();
                        return r.width > 0 && r.height > 0 && r.right > 0 && r.left < innerWidth;
                    }).length,
                    mainMenuIds: document.querySelectorAll('#main_menu').length
                });
            }
            """);
    }
}
