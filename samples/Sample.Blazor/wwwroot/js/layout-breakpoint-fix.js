// ---------------------------------------------------------------------------
// WORKAROUND for an upstream Fluent UI Blazor v5.0.0 bug. SAFE TO DELETE.
//
// FluentLayout decides whether it is in mobile mode in .NET (the internal
// FluentLayout.IsMobile property), and the only thing that ever sets it is the JS callback
// FluentLayout_MediaChangedAsync. The library's ResizeObserver calls that callback ONLY when
// it observes a TRANSITION (taken from the shipped v5.0.0 bundle):
//
//     new ResizeObserver(() => {
//         const wasMobile = el.hasAttribute("mobile");
//         const isMobile  = el.offsetWidth <= breakdownWidth;
//         if (!wasMobile && isMobile) { el.setAttribute("mobile", ""); helper?.invokeMethodAsync("FluentLayout_MediaChangedAsync", "mobile"); }
//         else if (wasMobile && !isMobile) { el.removeAttribute("mobile"); helper?.invokeMethodAsync("FluentLayout_MediaChangedAsync", "desktop"); }
//     }).observe(el);
//
// A page that is LOADED at or below the breakpoint (768px) therefore loses the notification,
// because of a race with the Blazor circuit:
//   1. Startup.afterStarted() -> LayoutAutoInitialize() -> Initialize(null, id) creates the
//      ResizeObserver with fluentLayoutInitialized = true and NO .NET helper attached.
//   2. The observer's single initial callback fires, sets the "mobile" attribute, and its
//      helper?.invokeMethodAsync(...) is a no-op because there is no helper yet.
//   3. FluentLayout.OnAfterRenderAsync then calls Initialize(helper, id), which hits the
//      fluentLayoutInitialized early-return: it attaches the helper but never re-creates the
//      observer, so the (already correct) DOM state is never reported to .NET.
//   4. Net result: the DOM is in mobile mode while .NET keeps IsMobile == false, so the
//      navigation is rendered into the CSS-hidden [area="nav"] column and the hamburger drawer
//      is rendered empty - a mobile user cannot navigate anywhere.
//
// Crossing the breakpoint at run time is unaffected (by then the helper is attached and the
// observer sees a real transition), which is what proves that this is an initialization race.
//
// This module hands the library's own verdict to .NET once the circuit is live. It does not
// re-implement the breakpoint math: the "mobile" attribute that the library's own detection
// already put on the layout element is used as-is. The HEIGHT is nudged, never the width - the
// observer compares offsetWidth with the breakpoint, so a width nudge could transiently report
// the wrong mode.
//
// It is a no-op when there is no .fluent-layout, when the layout already reports desktop, or
// when the interactive helper never arrives; it runs at most once per page load; and it never
// throws. Delete this file together with its call in MainLayout.razor.cs once upstream fixes
// the initialization order.
// ---------------------------------------------------------------------------

const LAYOUT_SELECTOR = ".fluent-layout";
const MOBILE_ATTRIBUTE = "mobile";
const HELPER_ATTEMPTS = 20;
const HELPER_INTERVAL_MS = 50;
const NUDGE_PX = 0.5;
const APPLIED_FLAG = "_iSynergyLayoutBreakpointFixApplied";

/**
 * Waits (bounded) until FluentLayout.OnAfterRenderAsync has attached its DotNetObjectReference.
 * @param {Element} layout The .fluent-layout element.
 * @returns {Promise<boolean>} True as soon as the helper is available.
 */
async function waitForDotNetHelperAsync(layout) {
    for (let attempt = 0; attempt < HELPER_ATTEMPTS; attempt++) {
        if (layout._fluentDotNetHelper) {
            return true;
        }

        await new Promise(resolve => setTimeout(resolve, HELPER_INTERVAL_MS));
    }

    return false;
}

/**
 * Briefly changes the layout's height so the library's own ResizeObserver re-runs its
 * desktop/mobile detection, then restores it on the following frame. The restore has to wait
 * for a rendering update: restoring in the same task would be coalesced away and the observer
 * would never see a size change at all.
 * @param {Element} layout The .fluent-layout element.
 */
function rerunBreakpointDetection(layout) {
    const previousHeight = layout.style.height;
    layout.style.height = `calc(var(--layout-height) - ${NUDGE_PX}px)`;

    requestAnimationFrame(() => requestAnimationFrame(() => {
        if (previousHeight) {
            layout.style.height = previousHeight;
        } else {
            layout.style.removeProperty("height");
        }
    }));
}

/**
 * Reports the layout's current breakpoint to .NET when the library failed to do so during the
 * initial page load. No-op unless the layout is in mobile mode and interactive.
 */
export async function applyInitialBreakpointFix() {
    try {
        const layout = document.querySelector(LAYOUT_SELECTOR);

        if (!layout || layout[APPLIED_FLAG]) {
            return;
        }

        // One attempt per page load, even when the fix cannot be applied.
        layout[APPLIED_FLAG] = true;

        // FluentLayout.OnAfterRenderAsync attaches the helper in its own first render, which may
        // run before or after the layout host's; wait for it instead of assuming an order.
        if (!await waitForDotNetHelperAsync(layout)) {
            return;
        }

        // Desktop loads are already consistent: FluentLayout.IsMobile defaults to false, which is
        // what the DOM shows. Only an initial mobile load has to be reported - a later crossing of
        // the breakpoint is handled by the library itself.
        if (!layout.hasAttribute(MOBILE_ATTRIBUTE)) {
            return;
        }

        // Let the library's own detection run again so its internal bookkeeping stays consistent.
        rerunBreakpointDetection(layout);

        // ...and report what it decided. In v5.0.0 the detection alone is NOT enough (see the note
        // at the top): the observer only calls .NET on a transition and the attribute it set during
        // afterStarted() is already correct, so re-firing it produces no notification at all. This
        // is the same [JSInvokable] entry point with the same argument the observer would have used;
        // FluentLayout_MediaChangedAsync returns immediately when .NET already agrees, so an extra
        // call is harmless.
        await layout._fluentDotNetHelper.invokeMethodAsync("FluentLayout_MediaChangedAsync", MOBILE_ATTRIBUTE);
    } catch {
        // A workaround for an upstream bug must never break the app.
    }
}
