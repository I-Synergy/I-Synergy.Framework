using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// Item 7 of the v5 migration validation: the busy overlay. v4 rendered a <c>FluentProgressRing</c>;
/// v5 renamed it to <c>FluentSpinner</c>, so the rendered element name is itself the assertion.
/// </summary>
[TestClass]
public class BusyOverlayTests : SamplePageTest
{
    [TestMethod]
    public async Task BusyOverlay_IsHiddenByDefault_AndUsesFluentSpinner()
    {
        await NavigateAsync(SampleDom.HomeRoute);

        // v4 element name; must not be rendered any more.
        Assert.AreEqual(0, await CountAsync(SampleDom.RemovedProgressRing), "fluent-progress-ring no longer exists in v5.");

        var busy = Page.Locator(SampleDom.BusyIndicator);
        Assert.AreEqual(1, await busy.CountAsync(), "#busyIndicator must exist.");
        await Expect(busy).ToBeHiddenAsync();

        Assert.AreEqual(1, await CountAsync($"{SampleDom.BusyIndicator} {SampleDom.Spinner}"),
            "The busy overlay must contain exactly one fluent-spinner.");

        var overlay = await Page.EvaluateAsync<string>(
            """
            () => {
                const indicator = document.getElementById('busyIndicator');
                const host = indicator?.closest('fluent-overlay');
                const dialog = host?.shadowRoot?.querySelector('dialog');
                return JSON.stringify({
                    hostId: host?.id,
                    attributes: host ? [...host.attributes].map(a => a.name) : null,
                    visibleAttribute: host?.hasAttribute('visible') ?? null,
                    shadowDialogOpen: dialog?.open ?? null,
                    indicatorRect: indicator ? { w: Math.round(indicator.getBoundingClientRect().width), h: Math.round(indicator.getBoundingClientRect().height) } : null,
                    spinnerDefined: !!customElements.get('fluent-spinner'),
                    spinnerRect: (() => { const s = document.querySelector('#busyIndicator fluent-spinner'); return s ? { w: Math.round(s.getBoundingClientRect().width), h: Math.round(s.getBoundingClientRect().height) } : null; })()
                });
            }
            """);

        Log($"busy overlay: {overlay}");
        Assert.AreEqual(0, await CountAsync(SampleDom.Spinner + ":visible"), "the busy spinner must not be visible by default.");

        await AssertBaselineAsync("busy-overlay");
    }

    /// <summary>
    /// The sample exposes no UI that flips <c>ICommonServices.BusyService.IsBusy</c>, so the "overlay
    /// becomes visible" path is exercised by toggling the <c>visible</c> attribute the component binds
    /// to. This proves the overlay + spinner pipeline works in this build; it does not replace an
    /// application-level test, because there is no application-level trigger to click.
    /// </summary>
    [TestMethod]
    public async Task BusyOverlay_ShowsAndHidesThroughItsVisibleAttribute()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await Expect(Page.Locator(SampleDom.BusyIndicator)).ToBeHiddenAsync();

        await Page.EvaluateAsync(
            "() => { document.getElementById('busyIndicator').closest('fluent-overlay').setAttribute('visible', 'true'); }");

        var busy = Page.Locator(SampleDom.BusyIndicator);
        await Expect(busy).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 10_000 });
        await Expect(Page.Locator($"{SampleDom.BusyIndicator} {SampleDom.Spinner}")).ToBeVisibleAsync();

        Log("busy overlay became visible and its fluent-spinner rendered.");

        await Page.EvaluateAsync(
            "() => { document.getElementById('busyIndicator').closest('fluent-overlay').removeAttribute('visible'); }");
        await Expect(busy).ToBeHiddenAsync();
    }
}
