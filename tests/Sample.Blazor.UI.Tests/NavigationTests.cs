using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// Items 2 and 3 of the v5 migration validation: the navigation renders real <c>FluentNav</c> /
/// <c>FluentNavItem</c> markup (not the v4 <c>fluent-nav-link</c> that v5 no longer styles or wires
/// up), exactly one copy of the nav exists in the DOM, and every route is reachable with exactly one
/// active item.
/// </summary>
[TestClass]
public class NavigationTests : SamplePageTest
{
    private static readonly string[] ExpectedItems = ["Home", "Counter", "Weather", "Command Demo"];

    [TestMethod]
    public async Task Navigation_RendersFourFluentNavItems_AndNoV4NavLinkMarkup()
    {
        await NavigateAsync(SampleDom.HomeRoute);

        await Expect(Page.Locator(SampleDom.NavRoot)).ToHaveCountAsync(1);
        await Expect(Page.Locator(SampleDom.NavItem)).ToHaveCountAsync(4);

        var texts = await Page.Locator(SampleDom.NavItem).AllInnerTextsAsync();
        CollectionAssert.AreEqual(
            ExpectedItems,
            texts.Select(t => t.Trim()).ToArray(),
            $"Navigation items were '{string.Join(" | ", texts)}'.");

        // Regression guard: v4 emitted <a class="fluent-nav-link">. v5 renders <a class="fluent-navitem">.
        // If the migration were incomplete the anchors would silently keep the old class and lose all
        // styling and keyboard handling.
        await Expect(Page.Locator(SampleDom.RemovedNavLinkClass)).ToHaveCountAsync(0);
        await Expect(Page.Locator(SampleDom.RemovedNavMenuTag)).ToHaveCountAsync(0);

        Log($"navigation: {await DescribeNavigationAsync()}");
        await AssertBaselineAsync("nav");
    }

    [TestMethod]
    public async Task Navigation_ContainsExactlyOneMainMenuId()
    {
        await NavigateAsync(SampleDom.HomeRoute);

        // The FluentLayout hamburger reuses the navigation area's ChildContent. Without
        // NavigationDeferredLoading the nav (and its element ids) would be rendered twice, and the
        // library's keyboard JS would bind to the first copy only.
        Assert.AreEqual(1, await CountAsync(SampleDom.NavMenuId), "id=\"main_menu\" must appear exactly once.");
        Assert.AreEqual(1, await CountAsync("nav"), "exactly one <nav> element expected.");
        Assert.AreEqual(1, await CountAsync(SampleDom.NavRoot), "exactly one nav.fluent-nav expected.");

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

        await Expect(active).ToHaveTextAsync(new Regex(expectedItem));
        await Expect(active).ToHaveAttributeAsync("aria-current", "page");

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
