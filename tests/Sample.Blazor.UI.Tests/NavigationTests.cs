using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// The navigation contract of the sample shell: four route links inside <c>nav.app-nav#main_menu</c>,
/// exactly one copy of the navigation in the DOM (no duplicated prerender), no leftover Fluent UI v4 /
/// v5 layout or nav markup, and every route reachable with exactly one active item.
/// <para>
/// Re-baselined for the hand-rolled shell: the navigation is no longer <c>FluentNav</c> /
/// <c>FluentNavItem</c> (<c>nav.fluent-nav</c> / <c>a.fluent-navitem</c>) with a <c>FluentLayout</c>
/// navigation area, but plain <c>NavLink</c>s in <c>nav.app-nav</c> inside <c>aside.app-sidebar</c>.
/// The removed v4 markup is still asserted to be absent — that guard is what catches a leftover copy of
/// the old markup, which would be unstyled and unwired.
/// </para>
/// </summary>
[TestClass]
public class NavigationTests : SamplePageTest
{
    private static readonly string[] ExpectedItems = ["Home", "Counter", "Weather", "Command Demo"];

    [TestMethod]
    public async Task Navigation_RendersFourNavItems_AndNoV4OrFluentLayoutMarkup()
    {
        await NavigateAsync(SampleDom.HomeRoute);

        await Expect(Page.Locator(SampleDom.NavRoot)).ToHaveCountAsync(1);
        await Expect(Page.Locator(SampleDom.NavItem)).ToHaveCountAsync(4);

        // Asserted on the label span, because the leading icon span (aria-hidden decoration) is part of
        // the anchor's text.
        var texts = await Page.Locator(SampleDom.NavItemLabel).AllInnerTextsAsync();
        CollectionAssert.AreEqual(
            ExpectedItems,
            texts.Select(t => t.Trim()).ToArray(),
            $"Navigation items were '{string.Join(" | ", texts)}'.");

        // Regression guards, from oldest to newest:
        // - v4 emitted <a class="fluent-nav-link"> and a <fluent-nav-menu> element; v5 emits neither.
        // - v5 emitted <nav class="fluent-nav"> inside a <div class="fluent-layout-item" area="nav">;
        //   the hand-rolled shell emits neither, so both must be gone from the page.
        await Expect(Page.Locator(SampleDom.RemovedNavLinkClass)).ToHaveCountAsync(0);
        await Expect(Page.Locator(SampleDom.RemovedNavMenuTag)).ToHaveCountAsync(0);
        await Expect(Page.Locator(SampleDom.RemovedNavRoot)).ToHaveCountAsync(0);
        await Expect(Page.Locator(SampleDom.RemovedLayoutItem)).ToHaveCountAsync(0);

        Log($"navigation: {await DescribeNavigationAsync()}");
        await AssertBaselineAsync("nav");
    }

    [TestMethod]
    public async Task Navigation_ContainsExactlyOneMainMenuId()
    {
        await NavigateAsync(SampleDom.HomeRoute);

        // Prerendering plus the interactive render can duplicate the shell for a moment (and a second
        // navigation copy would make the library-free JS/keyboard handling bind to a stale element).
        // Exactly one of each therefore has to be in the DOM.
        Assert.AreEqual(1, await CountAsync(SampleDom.NavMenuId), "id=\"main_menu\" must appear exactly once.");
        Assert.AreEqual(1, await CountAsync("nav"), "exactly one <nav> element expected.");
        Assert.AreEqual(1, await CountAsync(SampleDom.NavRoot), "exactly one nav.app-nav expected.");
        Assert.AreEqual(1, await CountAsync(SampleDom.LayoutRoot), "exactly one .app-shell expected.");
        Assert.AreEqual(1, await CountAsync(SampleDom.AreaSidebar), "exactly one sidebar expected.");
        Assert.AreEqual(1, await CountAsync(SampleDom.NavToggle), "exactly one nav toggle expected.");

        await Expect(Page.Locator(SampleDom.NavMenuId)).ToHaveAttributeAsync("role", "navigation");
        Log($"nav structure: {await DescribeNavigationAsync()}");
    }

    [TestMethod]
    [DataRow(SampleDom.HomeRoute, "Home")]
    [DataRow(SampleDom.CounterRoute, "Counter")]
    [DataRow(SampleDom.WeatherRoute, "Weather")]
    [DataRow(SampleDom.CommandDemoRoute, "Command Demo")]
    public async Task Navigation_EachRouteHasExactlyOneActiveItem(string route, string expectedItem)
    {
        await NavigateAsync(route);

        var active = Page.Locator(SampleDom.NavItemActive);
        Assert.AreEqual(1, await active.CountAsync(), $"{route}: expected exactly one active nav item.");

        // NavLink renders the 'active' class (the CSS hook) together with aria-current="page".
        await Expect(active).ToHaveTextAsync(new Regex(expectedItem));
        await Expect(active).ToHaveAttributeAsync(SampleDom.NavItemActiveAttribute, "page");

        Log($"{route}: active item = '{await active.InnerTextAsync()}' ({await DescribeNavigationAsync()})");
    }

    [TestMethod]
    public async Task Navigation_ClickingEachItemNavigatesToTheRoute()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await Expect(Page.Locator("h1")).ToHaveTextAsync("Hello, world!");

        // Counter
        await ClickNavItemAsync("Counter");
        await Expect(Page).ToHaveURLAsync(new Regex("/counter$"));
        await Expect(Page.Locator("h1")).ToHaveTextAsync("Counter");
        Assert.AreEqual(1, await CountAsync(SampleDom.NavItemActive), "/counter: exactly one active item.");

        // Weather (streamed rendering: wait for the 5 rows, then assert the nav)
        await ClickNavItemAsync("Weather");
        await Expect(Page).ToHaveURLAsync(new Regex("/weather$"));
        await Expect(Page.Locator("h1")).ToHaveTextAsync("Weather Sample");
        Assert.AreEqual(1, await CountAsync(SampleDom.NavItemActive), "/weather: exactly one active item.");

        // Command demo
        await ClickNavItemAsync("Command Demo");
        await Expect(Page).ToHaveURLAsync(new Regex("/commanddemoview$"));
        // Same outline level as every other route: the page heading is an h1 (strict mode also proves
        // there is exactly one such heading on the page).
        await Expect(Page.Locator("h1")).ToHaveTextAsync("Command Binding Demo");
        Assert.AreEqual(1, await CountAsync(SampleDom.NavItemActive), "/commanddemoview: exactly one active item.");

        // Back home
        await ClickNavItemAsync("Home");
        await Expect(Page).ToHaveURLAsync(new Regex("/$"));
        await Expect(Page.Locator("h1")).ToHaveTextAsync("Hello, world!");
        Assert.AreEqual(1, await CountAsync(SampleDom.NavItemActive), "/: exactly one active item.");

        Log($"after click-through navigation: {await DescribeNavigationAsync()}");
        await AssertBaselineAsync("nav-clicks");
    }

    private async Task ClickNavItemAsync(string text)
    {
        var item = Page.Locator(SampleDom.NavItem).Filter(new LocatorFilterOptions { HasTextString = text });
        Assert.AreEqual(1, await item.CountAsync(), $"No unique nav item matching '{text}'.");
        await item.ClickAsync();
    }
}
