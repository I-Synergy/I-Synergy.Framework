using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// The page shell contract: the sample renders its hand-rolled app shell (sidebar, breadcrumb header,
/// content column), the Fluent UI providers are still mounted, the removed v4 theming script is gone,
/// and no page produces request, console or Blazor-error noise.
/// <para>
/// Re-baselined: the shell used to be built from <c>FluentLayout</c> / <c>FluentLayoutItem</c>. Those
/// components are gone (their chrome does not match the native samples), so the shape assertions below
/// target the markup that actually exists now — and are equally specific: one shell, one of each area,
/// the 56px header, the fixed 300px desktop sidebar and a hamburger that is <c>display:none</c> on
/// desktop. What is still Fluent UI keeps its <c>fluent-*</c> assertions.
/// </para>
/// </summary>
[TestClass]
public class SmokeTests : SamplePageTest
{
    [TestMethod]
    public async Task Home_RendersTheHandRolledAppShell()
    {
        var response = await NavigateAsync(SampleDom.HomeRoute);

        Assert.IsNotNull(response, "No HTTP response for the home page.");
        Assert.AreEqual(200, response.Status, "Home page did not return HTTP 200.");

        await Expect(Page.Locator("h1")).ToHaveTextAsync("Hello, world!");

        // div.app-shell is the flex root of the shell (plain markup, not a custom element).
        await Expect(Page.Locator(SampleDom.LayoutRoot)).ToHaveCountAsync(1);
        await Expect(Page.Locator(SampleDom.LayoutRoot)).ToBeVisibleAsync();

        foreach (var area in new[] { SampleDom.AreaHeader, SampleDom.AreaSidebar, SampleDom.AreaContent, SampleDom.AreaFooter })
        {
            var locator = Page.Locator(area);
            Assert.AreEqual(1, await locator.CountAsync(), $"shell area '{area}' must exist exactly once");
            await Expect(locator).ToBeVisibleAsync();
        }

        // The pages render into the content column's article.
        await Expect(Page.Locator(SampleDom.AreaContent).Locator(SampleDom.ContentArticle)).ToHaveCountAsync(1);

        // The hamburger lives in the header, next to the breadcrumb and the header actions.
        await Expect(Page.Locator(SampleDom.AreaHeader).Locator(SampleDom.NavToggle)).ToHaveCountAsync(1);
        await Expect(Page.Locator(SampleDom.AreaHeader).Locator(SampleDom.Breadcrumb)).ToHaveCountAsync(1);

        var shell = await Page.EvaluateAsync<string>(
            """
            () => {
                const shell = document.querySelector('.app-shell');
                const header = document.querySelector('header.app-header');
                const sidebar = document.querySelector('aside.app-sidebar');
                const content = document.querySelector('main.app-content');
                const footer = document.querySelector('.app-sidebar-footer');
                const toggle = document.querySelector('button.app-nav-toggle');
                const box = (el) => { const r = el.getBoundingClientRect(); return { w: Math.round(r.width), h: Math.round(r.height) }; };
                return JSON.stringify({
                    shellTag: shell?.tagName.toLowerCase(),
                    shellTheme: shell?.getAttribute('data-app-theme'),
                    shellHeight: box(shell).h,
                    viewportHeight: innerHeight,
                    areas: [...document.querySelectorAll('.app-header, .app-sidebar, .app-content, .app-sidebar-footer')]
                        .map(a => a.className),
                    headerDisplay: getComputedStyle(header).display,
                    headerHeight: getComputedStyle(header).height,
                    sidebarPosition: getComputedStyle(sidebar).position,
                    sidebarWidth: box(sidebar).w,
                    contentDisplay: getComputedStyle(content).display,
                    footerChildCount: footer?.children.length ?? null,
                    navToggleDisplay: toggle ? getComputedStyle(toggle).display : null
                });
            }
            """);
        Log($"shell: {shell}");

        var state = System.Text.Json.JsonDocument.Parse(shell).RootElement;

        // The shell fills the viewport (100dvh) and is themed through a data attribute.
        Assert.AreEqual(state.GetProperty("viewportHeight").GetInt32(), state.GetProperty("shellHeight").GetInt32(),
            $"The shell must fill the viewport. Observed: {shell}");
        CollectionAssert.Contains(new[] { "dark", "light" }, state.GetProperty("shellTheme").GetString(),
            $"The shell must declare its theme. Observed: {shell}");

        // Desktop shape: permanent 300px sidebar column, no hamburger.
        Assert.AreEqual("flex", state.GetProperty("headerDisplay").GetString(), $"header.app-header must be laid out. Observed: {shell}");
        Assert.AreEqual("56px", state.GetProperty("headerHeight").GetString(), $"header.app-header must be 56px tall. Observed: {shell}");
        Assert.AreEqual("static", state.GetProperty("sidebarPosition").GetString(),
            $"The sidebar must be a permanent column on desktop. Observed: {shell}");
        Assert.AreEqual(300, state.GetProperty("sidebarWidth").GetInt32(),
            $"The desktop sidebar must be 300px wide. Observed: {shell}");
        Assert.AreEqual("none", state.GetProperty("navToggleDisplay").GetString(),
            $"Desktop has no collapsing toggle: the toggle must be display:none. Observed: {shell}");

        Assert.IsTrue(state.GetProperty("footerChildCount").GetInt32() >= 2,
            $"The sidebar footer must carry the Help link and the version label. Observed: {shell}");
        await Expect(Page.Locator(SampleDom.SidebarFooterNavItem)).ToHaveCountAsync(1);

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
