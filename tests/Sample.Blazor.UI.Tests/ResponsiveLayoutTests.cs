using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// Items 8 and 9 of the v5 migration validation: the responsive layout.
/// <para>
/// FluentLayout hides the navigation area below its breakpoint (<c>MobileBreakdownWidth</c>, 768px) and
/// the hamburger opens a drawer. <c>NavigationDeferredLoading="true"</c> means the navigation area and
/// the drawer render the same content *mutually exclusively* — the navigation area is dropped from the
/// DOM while the layout is in mobile mode and the hamburger drawer renders it instead. These tests
/// check that relocation actually happens, i.e. that a mobile user can reach the navigation.
/// </para>
/// </summary>
[TestClass]
public class ResponsiveLayoutTests : SamplePageTest
{
    private const int MobileWidth = 480;
    private const int MobileHeight = 800;

    [TestMethod]
    public async Task Mobile_At480px_TheNavigationIsRelocatedIntoTheHamburgerDrawer()
    {
        await OpenMobileHomeAsync();

        var beforeClick = await DescribeResponsiveStateAsync();
        Log($"mobile @480px before hamburger click: {beforeClick}");

        var hamburger = HamburgerButton();
        await Expect(hamburger).ToBeVisibleAsync();
        await hamburger.ClickAsync();

        await WaitForDrawerOpenAsync();

        var afterClick = await DescribeResponsiveStateAsync();
        Log($"mobile @480px after hamburger click: {afterClick}");

        var state = System.Text.Json.JsonDocument.Parse(afterClick).RootElement;

        // Preconditions that do hold — asserted first so the failure below points at the real defect.
        Assert.IsTrue(state.GetProperty("layoutMobileAttribute").GetBoolean(),
            $"The layout never switched to mobile mode. Observed DOM: {afterClick}");
        // The navigation column must not be *usable* at mobile width. Two DOM shapes satisfy that and
        // both are accepted here: the column is gone from the DOM (the v5 behaviour - the same content
        // is rendered into the hamburger drawer instead by NavigationDeferredLoading), or it is still
        // rendered but the mobile CSS hides it *and* it occupies a 0x0 box. Anything that keeps a real
        // box fails, i.e. a column the user could still see, click or tab into counts as a defect even
        // when it is only present before the layout has settled.
        var navAreaPresent = state.GetProperty("navAreaPresent").GetBoolean();
        var navAreaDisplay = state.GetProperty("navAreaDisplay").GetString();
        var navAreaBox = state.GetProperty("navAreaBox");
        var navAreaHiddenByCss = string.Equals(navAreaDisplay, "none", StringComparison.OrdinalIgnoreCase);
        var navAreaHasNoBox = navAreaBox.ValueKind != System.Text.Json.JsonValueKind.Null
                              && navAreaBox.GetProperty("w").GetInt32() == 0
                              && navAreaBox.GetProperty("h").GetInt32() == 0;
        var navAreaIsUsable = navAreaPresent && !(navAreaHiddenByCss && navAreaHasNoBox);
        Assert.IsFalse(
            navAreaIsUsable,
            $"The navigation column is still usable at mobile width (present={navAreaPresent}, " +
            $"display={navAreaDisplay ?? "absent"}, box={navAreaBox}). It must be either absent from the DOM " +
            $"(the hamburger drawer carries the navigation) or hidden by the mobile CSS with a 0x0 box. " +
            $"Observed DOM: {afterClick}");
        Assert.IsTrue(state.GetProperty("drawerDialogOpen").GetBoolean(),
            $"The hamburger drawer did not open. Observed DOM: {afterClick}");

        // The defect: the drawer is empty and every navigation item is invisible, so a mobile user
        // cannot navigate anywhere.
        var drawerNavItems = state.GetProperty("navItemsInDrawer").GetInt32();
        var visibleNavItems = state.GetProperty("visibleNavItems").GetInt32();

        Assert.AreEqual(
            4,
            drawerNavItems,
            $"The hamburger drawer must contain the 4 navigation items after opening. Observed DOM: {afterClick}");
        Assert.AreNotEqual(
            0,
            visibleNavItems,
            $"No navigation item is visible while the drawer is open, so mobile navigation is unreachable. Observed DOM: {afterClick}");

        Assert.AreEqual(1, await CountAsync(SampleDom.NavMenuId), "The navigation must exist exactly once while the drawer is open.");

        await AssertBaselineAsync("mobile-drawer");
    }

    /// <summary>
    /// Root cause probe for the empty mobile drawer: load the page at desktop width first (so the
    /// layout's JS-to-.NET initialization race has already been won) and cross the breakpoint at
    /// runtime. If the navigation *does* relocate into the drawer this time, the .NET side works and
    /// the defect is a missed <c>FluentLayout_MediaChangedAsync</c> notification during initial load.
    /// </summary>
    [TestMethod]
    public async Task Mobile_Diagnosis_TheNavigationRelocatesOnAnObservedBreakpointTransition()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await WaitForLayoutInitialisedAsync();
        Log($"diagnosis: loaded at desktop width, layout initialised = {await DescribeLayoutJsAsync()}");

        await Page.SetViewportSizeAsync(MobileWidth, MobileHeight);
        await Page.WaitForFunctionAsync(
            "() => document.querySelector('.fluent-layout')?.hasAttribute('mobile') === true",
            null,
            new() { Timeout = 20_000 });

        // Give the .NET side the round trip it needs to re-render after the breakpoint callback.
        await Page.WaitForFunctionAsync(
            "() => !document.querySelector('.fluent-layout-item[area=\"nav\"]')",
            null,
            new() { Timeout = 15_000 });

        var state = await DescribeResponsiveStateAsync();
        Log($"diagnosis: after crossing the breakpoint at runtime: {state}");

        var parsed = System.Text.Json.JsonDocument.Parse(state).RootElement;
        Assert.IsFalse(parsed.GetProperty("navAreaPresent").GetBoolean(),
            $"The navigation area was not removed from the DOM after the runtime breakpoint change. Observed DOM: {state}");
        Assert.AreEqual(4, parsed.GetProperty("navItemsInDrawer").GetInt32(),
            $"The drawer did not receive the navigation after the runtime breakpoint change. Observed DOM: {state}");

        await AssertBaselineAsync("mobile-diagnosis");
    }

    [TestMethod]
    public async Task Mobile_At480px_TheUserCanNavigateFromTheDrawer()
    {
        await OpenMobileHomeAsync();

        await HamburgerButton().ClickAsync();
        await WaitForDrawerOpenAsync();

        var drawer = Drawer();
        var counterLink = drawer.Locator(SampleDom.NavItem).Filter(new LocatorFilterOptions { HasTextString = "Counter" });

        // Diagnostics only: the assertion below is rethrown unchanged. When the drawer never fills, the
        // recorded DOM tells the two possible causes apart - "the layout is still desktop in .NET, so the
        // navigation stayed in the [area=nav] column" (the JS breakpoint hand-off gave up) versus "the
        // drawer is open but .NET simply has not rendered the navigation into it yet".
        try
        {
            await Expect(counterLink).ToBeVisibleAsync();
        }
        catch
        {
            await TryLogResponsiveStateAsync("mobile drawer link missing");
            throw;
        }

        await counterLink.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/counter$"));
        await Expect(Page.Locator("h1")).ToHaveTextAsync("Counter");

        Log($"mobile navigation from the drawer reached {Page.Url}");
        await AssertBaselineAsync("mobile-navigation");
    }

    [TestMethod]
    public async Task Desktop_HamburgerCollapsesAndRestoresTheNavigation()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await WaitForLayoutInitialisedAsync();

        var navArea = Page.Locator(SampleDom.AreaNavigation);
        await Expect(navArea).ToBeVisibleAsync();
        Assert.IsNull(await navArea.GetAttributeAsync("hidden"), "The navigation area must start expanded on desktop.");

        await HamburgerButton().ClickAsync();
        await Expect(navArea).ToBeHiddenAsync();
        Assert.IsNotNull(await navArea.GetAttributeAsync("hidden"), "The first desktop click must collapse the navigation.");

        await HamburgerButton().ClickAsync();
        await Expect(navArea).ToBeVisibleAsync();
        Assert.IsNull(await navArea.GetAttributeAsync("hidden"), "The second desktop click must expand the navigation again.");

        await AssertBaselineAsync("desktop-collapse");
    }

    /// <summary>
    /// Item 9 — measurement, not a pass/fail judgement: reports whether the navigation column shows a
    /// background band below the (content height) navigation, as reported by a separate agent.
    /// </summary>
    [TestMethod]
    public async Task Desktop_NavColumn_ReportsColumnAndNavigationHeightsAndBackgrounds()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await WaitForLayoutInitialisedAsync();

        var measurements = await Page.EvaluateAsync<string>(
            """
            () => {
                const column = document.querySelector('.fluent-layout-item[area="nav"]');
                const nav = column?.querySelector('nav.fluent-nav');
                const article = document.getElementById('article');
                const layout = document.querySelector('.fluent-layout');
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
                    layout: describe(layout),
                    seamPx: (c && n) ? Math.round(c.height - n.height) : null,
                    sameBackground: (c && n) ? c.background === n.background : null,
                    navInlineStyle: nav?.getAttribute('style'),
                    navHasHeightParameter: column?.getAttribute('style')?.includes('height') ?? false
                });
            }
            """);

        Log($"nav column measurements: {measurements}");

        var state = System.Text.Json.JsonDocument.Parse(measurements).RootElement;
        Assert.AreNotEqual(System.Text.Json.JsonValueKind.Null, state.GetProperty("column").ValueKind, "The navigation column must exist.");
        Assert.AreNotEqual(System.Text.Json.JsonValueKind.Null, state.GetProperty("nav").ValueKind, "The navigation must exist.");

        var columnHeight = state.GetProperty("column").GetProperty("height").GetInt32();
        var navHeight = state.GetProperty("nav").GetProperty("height").GetInt32();
        var seam = state.GetProperty("seamPx").GetInt32();

        Assert.IsTrue(columnHeight > 0, "The navigation column has no height.");
        Assert.IsTrue(navHeight > 0, "The navigation has no height.");

        Console.WriteLine(
            $"[evidence] two-tone check: navigation column is {columnHeight}px tall, nav.fluent-nav is {navHeight}px tall, " +
            $"difference (background band below the nav) = {seam}px. " +
            $"column background={state.GetProperty("column").GetProperty("background").GetString()}, " +
            $"nav background={state.GetProperty("nav").GetProperty("background").GetString()}.");
    }

    private async Task OpenMobileHomeAsync()
    {
        await Page.SetViewportSizeAsync(MobileWidth, MobileHeight);
        await NavigateAsync(SampleDom.HomeRoute);

        // The layout JS flags mobile mode through a `mobile` attribute on .fluent-layout; waiting for it
        // avoids asserting on the pre-hydration (prerendered) DOM, which always contains the nav area.
        await Page.WaitForFunctionAsync(
            "() => document.querySelector('.fluent-layout')?.hasAttribute('mobile') === true",
            null,
            new() { Timeout = 20_000 });
    }

    private ILocator HamburgerButton() => Page.Locator($"{SampleDom.LayoutHamburger} fluent-button");

    private ILocator Drawer() => Page.Locator(SampleDom.Drawer);

    private async Task WaitForDrawerOpenAsync()
    {
        await Page.WaitForFunctionAsync(
            """
            () => {
                const hamburger = document.querySelector('.fluent-layout-hamburger');
                const drawer = hamburger ? document.getElementById(hamburger.id + '-drawer') : null;
                const dialog = drawer?.shadowRoot?.querySelector('dialog');
                return !!dialog?.open;
            }
            """,
            null,
            new() { Timeout = 15_000 });
    }

    /// <summary>
    /// Records the responsive DOM and the page diagnostics after a failure without ever replacing the
    /// failure itself.
    /// </summary>
    private async Task TryLogResponsiveStateAsync(string label)
    {
        try
        {
            Log($"{label}: {await DescribeResponsiveStateAsync()}");
            ReportDiagnostics(label);
        }
        catch
        {
            // Diagnostics must never mask the assertion they were collected for.
        }
    }

    private async Task<string> DescribeLayoutJsAsync()
    {
        return await Page.EvaluateAsync<string>(
            """
            () => {
                const layout = document.querySelector('.fluent-layout');
                const hamburger = document.querySelector('.fluent-layout-hamburger');
                return JSON.stringify({
                    layoutInitialized: layout?.fluentLayoutInitialized ?? null,
                    layoutHasDotNetHelper: !!layout?._fluentDotNetHelper,
                    layoutMobileAttribute: layout?.hasAttribute('mobile') ?? null,
                    layoutWidth: layout?.offsetWidth ?? null,
                    hamburgerInitialized: hamburger?.fluentHamburgerInitialized ?? null,
                    hamburgerHasDotNetHelper: !!hamburger?._fluentHamburgerDotNetHelper
                });
            }
            """);
    }

    private async Task<string> DescribeResponsiveStateAsync()
    {
        var layoutJs = await DescribeLayoutJsAsync();

        var dom = await Page.EvaluateAsync<string>(
            """
            () => {
                const layout = document.querySelector('.fluent-layout');
                const navArea = document.querySelector('.fluent-layout-item[area="nav"]');
                const hamburger = document.querySelector('.fluent-layout-hamburger');
                const drawer = hamburger ? document.getElementById(hamburger.id + '-drawer') : null;
                const dialog = drawer?.shadowRoot?.querySelector('dialog');
                const items = [...document.querySelectorAll('a.fluent-navitem')];
                const box = (el) => { const r = el.getBoundingClientRect(); return { w: Math.round(r.width), h: Math.round(r.height) }; };
                return JSON.stringify({
                    viewport: { w: innerWidth, h: innerHeight },
                    layoutMobileAttribute: layout?.hasAttribute('mobile') ?? null,
                    layoutWidth: layout?.offsetWidth ?? null,
                    navAreaPresent: !!navArea,
                    navAreaDisplay: navArea ? getComputedStyle(navArea).display : null,
                    navAreaBox: navArea ? box(navArea) : null,
                    navItemsInNavArea: navArea ? navArea.querySelectorAll('a.fluent-navitem').length : 0,
                    hamburgerDisplay: hamburger ? getComputedStyle(hamburger).display : null,
                    hamburgerAriaExpanded: hamburger?.getAttribute('aria-expanded') ?? null,
                    drawerPresent: !!drawer,
                    drawerDialogOpen: dialog?.open ?? null,
                    drawerBox: drawer ? box(drawer) : null,
                    navInDrawerCount: drawer ? drawer.querySelectorAll('nav.fluent-nav').length : 0,
                    navItemsInDrawer: drawer ? drawer.querySelectorAll('a.fluent-navitem').length : 0,
                    mainMenuIds: document.querySelectorAll('#main_menu').length,
                    totalNavItems: items.length,
                    visibleNavItems: items.filter(e => { const r = e.getBoundingClientRect(); return r.width > 0 && r.height > 0; }).length
                });
            }
            """);

        return $"{dom.TrimEnd('}')},\"layoutJs\":{layoutJs}}}";
    }
}
