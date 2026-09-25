using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// Item 1 of the v5 migration validation: the page shell is built from real v5 components
/// (<c>FluentLayout</c> / <c>FluentLayoutItem</c> / <c>FluentProviders</c>), the removed v4 theming
/// script is gone, and no page produces request, console or Blazor-error noise.
/// </summary>
[TestClass]
public class SmokeTests : SamplePageTest
{
    [TestMethod]
    public async Task Home_RendersTheFluentLayoutShell()
    {
        var response = await NavigateAsync(SampleDom.HomeRoute);

        Assert.IsNotNull(response, "No HTTP response for the home page.");
        Assert.AreEqual(200, response.Status, "Home page did not return HTTP 200.");

        await Expect(Page.Locator("h1")).ToHaveTextAsync("Hello, world!");

        // FluentLayout renders <div class="fluent-layout"> (class based, not a custom element).
        await Expect(Page.Locator(SampleDom.LayoutRoot)).ToHaveCountAsync(1);

        foreach (var area in new[] { "header", "nav", "content", "footer" })
        {
            var locator = Page.Locator(SampleDom.LayoutArea(area));
            Assert.AreEqual(1, await locator.CountAsync(), $"layout item [area={area}] must exist exactly once");
            await Expect(locator).ToBeVisibleAsync();
        }

        // The hamburger lives in the header area and the footer carries the 40px height parameter.
        await Expect(Page.Locator(SampleDom.AreaHeader).Locator(SampleDom.LayoutHamburger)).ToHaveCountAsync(1);
        await Expect(Page.Locator(SampleDom.AreaFooter)).ToHaveAttributeAsync("area", "footer");

        var shell = await Page.EvaluateAsync<string>(
            """
            () => JSON.stringify({
                layoutTag: document.querySelector('.fluent-layout')?.tagName.toLowerCase(),
                layoutCustomElement: !!customElements.get('fluent-layout'),
                areas: [...document.querySelectorAll('.fluent-layout-item')].map(i => i.getAttribute('area')),
                headerHeight: getComputedStyle(document.querySelector('.fluent-layout-item[area="header"]')).height,
                footerHeight: getComputedStyle(document.querySelector('.fluent-layout-item[area="footer"]')).height,
                hamburgerDisplay: getComputedStyle(document.querySelector('.fluent-layout-hamburger')).display
            })
            """);
        Log($"layout shell: {shell}");

        await AssertBaselineAsync("home");
    }

    [TestMethod]
    public async Task Home_RendersFluentProvidersContainingTheDialogProvider()
    {
        await NavigateAsync(SampleDom.HomeRoute);

        await Expect(Page.Locator(SampleDom.Providers)).ToHaveCountAsync(1);

        // The dialog provider must be *inside* the providers container, otherwise the dialog service
        // silently has nowhere to render into.
        await Expect(Page.Locator($"{SampleDom.Providers} {SampleDom.DialogProvider}")).ToHaveCountAsync(1);

        await Expect(Page.Locator($"{SampleDom.Providers} {SampleDom.ToastProvider}")).ToHaveCountAsync(1);
        await Expect(Page.Locator($"{SampleDom.Providers} {SampleDom.TooltipProvider}")).ToHaveCountAsync(1);

        // v4 emitted a FluentMenuProvider; it no longer exists in v5.
        await Expect(Page.Locator(".fluent-menu-provider")).ToHaveCountAsync(0);

        var providers = await Page.EvaluateAsync<string>(
            """
            () => JSON.stringify({
                providersTag: document.querySelector('.fluent-providers')?.tagName.toLowerCase(),
                children: [...document.querySelector('.fluent-providers').children].map(c => c.className)
            })
            """);
        Log($"providers: {providers}");

        await AssertBaselineAsync("providers");
    }

    [TestMethod]
    public async Task Home_DoesNotReferenceTheRemovedLoadingThemeScript()
    {
        await NavigateAsync(SampleDom.HomeRoute);

        var content = await Page.ContentAsync();
        Assert.AreEqual(
            0,
            CountOccurrences(content, "loading-theme"),
            "'loading-theme' still appears in the rendered HTML; v5 does not ship loading-theme.js so " +
            "this is a guaranteed 404.");

        var themeRequests = await Page.EvaluateAsync<string>(
            "() => JSON.stringify(performance.getEntriesByType('resource').map(r => r.name).filter(n => n.includes('loading-theme')))");
        Log($"resource entries mentioning loading-theme: {themeRequests}");
        Assert.AreEqual("[]", themeRequests, "The browser requested a script whose name contains 'loading-theme'.");

        await AssertBaselineAsync("loading-theme");
    }

    [TestMethod]
    public async Task EveryRoute_LoadsCleanlyWithNoFailedRequestsOrConsoleErrors()
    {
        foreach (var route in SampleDom.AllRoutes)
        {
            var response = await NavigateAsync(route);

            Assert.IsNotNull(response, $"No HTTP response for {route}.");
            Assert.IsTrue(
                response.Status is >= 200 and < 400,
                $"{route} returned HTTP {response.Status}.");

            await Expect(Page.Locator(SampleDom.BlazorErrorUi)).ToBeHiddenAsync();
            await Expect(Page.Locator(SampleDom.LayoutRoot)).ToHaveCountAsync(1);

            // AssertBaselineAsync() reports the whole diagnostic set for the failing route.
            await AssertBaselineAsync(route);

            Log($"{route} -> HTTP {response.Status}");
        }
    }

    private static int CountOccurrences(string haystack, string needle)
    {
        var count = 0;
        var index = haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
        while (index >= 0)
        {
            count++;
            index = haystack.IndexOf(needle, index + needle.Length, StringComparison.OrdinalIgnoreCase);
        }

        return count;
    }
}
