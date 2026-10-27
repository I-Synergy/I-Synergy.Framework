using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// Item 6 of the v5 migration validation — the highest risk area of the migration, previously only
/// verified statically: the dialog system. v5 removed <c>IDialogContentComponent</c>,
/// <c>FluentDialogHeader</c>, <c>FluentDialogFooter</c> and <c>FluentTextField</c>; this class proves
/// that opening a dialog through <c>IDialogService</c>, rendering its custom header/footer templates,
/// binding an input and cancelling all work in a live Blazor Server circuit.
/// </summary>
[TestClass]
public class DialogTests : SamplePageTest
{
    private const string DialogTitle = "Open Sample Dialog";
    private const string TypedText = "Migrated dialog text";

    [TestMethod]
    public async Task OpenSampleDialog_OpensAFluentDialog()
    {
        await OpenDialogAsync();

        Assert.AreEqual(1, await CountAsync(SampleDom.Dialog), "Exactly one fluent-dialog expected.");

        // The dialog must live inside the provider rendered by <FluentProviders>.
        Assert.AreEqual(
            1,
            await CountAsync($"{SampleDom.DialogProvider} {SampleDom.Dialog}"),
            "The dialog was not rendered inside .fluent-dialog-provider.");

        // The <fluent-dialog> host element itself has a zero sized box (the panel is rendered by the web
        // component's shadow DOM <dialog>), so Playwright reports the host as "hidden" even while the
        // dialog is open and interactive. Visibility is therefore asserted on the rendered content.
        var dialogBody = Page.Locator($"{SampleDom.Dialog} fluent-dialog-body");
        await Expect(dialogBody).ToHaveCountAsync(1);
        await Expect(Page.Locator($"{SampleDom.Dialog} [slot='title']")).ToBeVisibleAsync();
        await Expect(Page.Locator($"{SampleDom.Dialog} {SampleDom.TextInput} input")).ToBeVisibleAsync();

        Log($"dialog dom: {await DescribeDialogAsync()}");
        await AssertBaselineAsync("dialog-open");
    }

    [TestMethod]
    public async Task Dialog_RendersAFluentTextInputInsteadOfTheRemovedTextField()
    {
        await OpenDialogAsync();

        // FluentTextField (v4) no longer exists; the migrated markup uses FluentTextInput.
        Assert.AreEqual(1, await CountAsync($"{SampleDom.Dialog} {SampleDom.TextInput}"),
            "The dialog body must contain exactly one fluent-text-input.");
        Assert.AreEqual(0, await CountAsync($"{SampleDom.Dialog} {SampleDom.RemovedTextField}"),
            "v4's fluent-text-field must not appear.");

        var input = Page.Locator($"{SampleDom.Dialog} {SampleDom.TextInput} input");
        await Expect(input).ToBeVisibleAsync();
        await Expect(input).ToBeEditableAsync();

        Log($"text input: {await Page.Locator($"{SampleDom.Dialog} {SampleDom.TextInput}").EvaluateAsync<string>("e => e.outerHTML")}");
    }

    [TestMethod]
    public async Task Dialog_ShowsTheConfiguredHeaderTitle()
    {
        await OpenDialogAsync();

        var title = Page.Locator($"{SampleDom.Dialog} [slot='title']");
        await Expect(title).ToContainTextAsync(DialogTitle);

        Log($"dialog title slot: '{await title.InnerTextAsync()}'");
    }

    [TestMethod]
    public async Task Dialog_TextInputBindingKeepsTheTypedValue()
    {
        await OpenDialogAsync();

        var input = Page.Locator($"{SampleDom.Dialog} {SampleDom.TextInput} input");
        await input.PressSequentiallyAsync(TypedText, new() { Delay = 25 });

        // A broken @bind-Value re-renders the component with the (empty) view model value and wipes
        // what was typed; a working binding round-trips every keystroke.
        await Expect(input).ToHaveValueAsync(TypedText);

        // Force a Blazor re-render and re-check: press Tab (blur) and click a neutral part of the dialog.
        await input.PressAsync("Tab");
        await Page.Locator($"{SampleDom.Dialog} [slot='title']").ClickAsync();
        await Task.Delay(300);
        await Expect(input).ToHaveValueAsync(TypedText);

        Log($"text input value after typing + blur + re-render: '{await input.InputValueAsync()}'");
    }

    /// <summary>
    /// Regression guard for a second, previously unfixed defect in the dialog header: a freshly created
    /// <c>Budget</c> carries <c>default(DateTimeOffset)</c>, which used to be interpolated into the
    /// subtitle and rendered a second header line reading "1/1/0001 12:00:00 AM +00:00" under the title.
    /// The title slot must contain exactly the title and nothing else.
    /// </summary>
    [TestMethod]
    public async Task Dialog_HeaderShowsOnlyTheTitleLine_WithoutTheDefaultDateSubtitle()
    {
        await OpenDialogAsync();

        var title = Page.Locator(SampleDom.DialogTitleSlot);
        await Expect(title).ToContainTextAsync(DialogTitle);

        var text = await title.InnerTextAsync();
        var normalized = Regex.Replace(text, @"\s+", " ").Trim();
        var headerDom = await title.EvaluateAsync<string>("t => t.innerHTML");
        Log($"dialog title slot text: '{normalized}' (raw: '{text.Replace(Environment.NewLine, "\\n")}')");
        Log($"dialog header dom: {headerDom}");

        // Structural half of "only the title line": BaseModelDialog renders the title and the (optional)
        // subtitle as sibling lines; a second line means the default-date subtitle came back.
        Assert.AreEqual(
            1,
            await Page.Locator($"{SampleDom.DialogTitleSlot} fluent-text").CountAsync(),
            $"The dialog header must render exactly one line (the title), but rendered more. " +
            $"Observed header dom: {headerDom}");

        Assert.IsFalse(
            normalized.Contains("0001", StringComparison.Ordinal),
            $"The dialog header still renders the default-date subtitle: a fresh Budget must not render " +
            $"'{normalized}'. Observed header text: '{normalized}'.");
        Assert.IsFalse(
            normalized.Contains("12:00:00 AM", StringComparison.Ordinal),
            $"The dialog header still renders the default-date subtitle line. Observed header text: '{normalized}'.");

        // "Only the title line": after whitespace normalisation the header must be the title, verbatim -
        // any second line (subtitle) would make this differ.
        Assert.AreEqual(
            DialogTitle,
            normalized,
            $"The dialog header must render only the title line, but rendered '{normalized}'.");
    }

    /// <summary>
    /// Regression guard for the dialog panel size. v5.0.0 ships no panel height for a centred dialog, so
    /// the shadow-DOM <c>&lt;dialog part="dialog"&gt;</c> used to stretch to the full viewport (600x900
    /// around a ~200px body), leaving a large empty area under the buttons. The panel must be sized by
    /// its content instead.
    /// </summary>
    [TestMethod]
    public async Task Dialog_PanelIsContentHeight_AndNotStretchedToTheViewport()
    {
        await OpenDialogAsync();
        await WaitForDialogAnimationAsync();

        var measurements = await Page.Locator(SampleDom.Dialog).EvaluateAsync<string>(DialogPanelMetricsScript);
        Log($"dialog panel measurements: {measurements}");

        var state = System.Text.Json.JsonDocument.Parse(measurements).RootElement;
        Assert.IsFalse(
            state.TryGetProperty("error", out var probeError),
            $"The dialog panel could not be measured: {probeError}. Measurements: {measurements}");

        var viewportHeight = state.GetProperty("viewport").GetProperty("h").GetInt32();
        var panel = state.GetProperty("panel");
        var panelHeight = panel.GetProperty("h").GetInt32();
        var contentHeight = state.GetProperty("contentHeight").GetInt32();
        var emptyBelowContent = state.GetProperty("emptyBelowContent").GetInt32();
        var emptyAboveContent = state.GetProperty("emptyAboveContent").GetInt32();

        // The panel contains its content (a collapsed panel is a different defect, not a pass)...
        Assert.IsTrue(panelHeight > 0, $"The dialog panel has no height. Measurements: {measurements}");
        Assert.IsTrue(
            contentHeight > 0,
            $"The dialog panel rendered no measurable content (title/input/actions). Measurements: {measurements}");
        Assert.IsTrue(
            panelHeight >= contentHeight,
            $"The dialog panel ({panelHeight}px) is shorter than its own content ({contentHeight}px). " +
            $"Measurements: {measurements}");

        // ...and it is materially shorter than the viewport. The defect rendered the panel at the full
        // viewport height (900px at this viewport); the fix sizes it to its content (~210px). The bound
        // is deliberately relative so it does not hard-code the fixed height.
        Assert.IsTrue(
            panelHeight < viewportHeight * MaxPanelHeightToViewportRatio,
            $"The dialog panel is not content-sized: {panelHeight}px against a {viewportHeight}px viewport " +
            $"(ratio {panelHeight / (double)viewportHeight:F2}, allowed < {MaxPanelHeightToViewportRatio:F2}). " +
            $"Measurements: {measurements}");

        // No large empty area below the body/actions - that empty strip is what the oversized panel
        // looked like. Only the panel's own bottom padding may remain.
        Assert.IsTrue(
            emptyBelowContent <= MaxPanelPaddingPx,
            $"The dialog panel leaves {emptyBelowContent}px of empty space below its last content " +
            $"(body/actions) - more than the panel's own {MaxPanelPaddingPx}px bottom padding allows. " +
            $"Measurements: {measurements}");
        Assert.IsTrue(
            emptyAboveContent <= MaxPanelPaddingPx,
            $"The dialog panel leaves {emptyAboveContent}px of empty space above its first content. " +
            $"Measurements: {measurements}");
    }

    [TestMethod]
    public async Task Dialog_RendersCancelAndSaveButtons_AndCancelClosesTheDialog()
    {
        await OpenDialogAsync();

        var cancel = Page.Locator($"{SampleDom.Dialog} fluent-button").Filter(new LocatorFilterOptions { HasTextString = "Cancel" });
        Assert.AreEqual(1, await cancel.CountAsync(), "The dialog must render a Cancel button.");

        // v4 labelled the submit action "Save"; the migrated dialog keeps BaseModelDialog's text.
        var submit = Page.Locator($"{SampleDom.Dialog} fluent-button").Filter(new LocatorFilterOptions { HasTextString = "Add" });
        Assert.AreEqual(1, await submit.CountAsync(),
            $"The dialog must render its submit button. Buttons found: {await DescribeButtonsAsync()}");
        await Expect(submit).ToBeVisibleAsync();

        var buttons = await DescribeButtonsAsync();
        Log($"dialog buttons: {buttons}");

        await cancel.ClickAsync();

        await Expect(Page.Locator(SampleDom.Dialog)).ToHaveCountAsync(0);
        Log("dialog closed after Cancel.");
    }

    private async Task OpenDialogAsync()
    {
        await NavigateAsync(SampleDom.CommandDemoRoute);

        var open = Page.GetByRole(AriaRole.Button, new() { Name = "Open Sample Dialog" });
        await Expect(open).ToBeVisibleAsync();
        await open.ClickAsync();

        await Expect(Page.Locator(SampleDom.Dialog)).ToHaveCountAsync(1);
    }

    private async Task<string> DescribeDialogAsync()
    {
        // Note: this runs as page JavaScript, so it cannot see into shadow roots — the dialog's <input>
        // and its <label> live inside the fluent-text-input shadow DOM and are only reachable through
        // Playwright locators (see Dialog_RendersAFluentTextInputInsteadOfTheRemovedTextField).
        return await Page.Locator(SampleDom.DialogProvider).EvaluateAsync<string>(
            """
            (provider) => JSON.stringify({
                providerChildren: [...provider.children].map(c => c.tagName.toLowerCase() + '.' + c.className + '[id=' + c.id + ']'),
                dialogs: [...provider.querySelectorAll('fluent-dialog')].map(d => ({
                    id: d.id,
                    attributes: [...d.attributes].map(a => a.name + '=' + a.value),
                    hostBox: { w: Math.round(d.getBoundingClientRect().width), h: Math.round(d.getBoundingClientRect().height) },
                    shadowDialogOpen: d.shadowRoot?.querySelector('dialog')?.open ?? 'no shadow root',
                    shadowDialogBox: (() => { const s = d.shadowRoot?.querySelector('dialog'); if (!s) return null; const r = s.getBoundingClientRect(); return { w: Math.round(r.width), h: Math.round(r.height) }; })(),
                    slots: [...d.querySelectorAll('[slot]')].map(s => s.getAttribute('slot') + ':' + s.textContent.trim().slice(0, 40)),
                    textInputs: d.querySelectorAll('fluent-text-input').length,
                    buttons: [...d.querySelectorAll('fluent-button')].map(b => b.textContent.trim())
                }))
            })
            """);
    }

    private async Task<string> DescribeButtonsAsync()
    {
        return await Page.Locator($"{SampleDom.Dialog} fluent-button").AllInnerTextsAsync()
            .ContinueWith(t => string.Join(" | ", t.Result.Select(b => b.Trim())));
    }

    /// <summary>
    /// Waits for the dialog's open animation to settle. The panel belongs to a web component and scales
    /// in: measured mid-animation it is 542x192 instead of its settled 600x212, so boxes taken before the
    /// animation finishes are not the layout the user ends up with.
    /// </summary>
    private async Task WaitForDialogAnimationAsync()
    {
        await Page.Locator(SampleDom.Dialog).EvaluateAsync<string>(
            """
            async (dialog) => {
                const panel = dialog.shadowRoot?.querySelector('dialog');
                if (!panel) return 'no panel';
                await Promise.all(panel.getAnimations({ subtree: true })
                    .map(animation => animation.finished.catch(() => undefined)));
                return 'settled';
            }
            """);
    }

    /// <summary>
    /// Upper bound for how tall the panel may be relative to the viewport. The defect rendered it at
    /// exactly 1.0x (the full 900px viewport); a content-sized panel stays far below this bound.
    /// </summary>
    private const double MaxPanelHeightToViewportRatio = 0.75;

    /// <summary>
    /// Slack, in px, for the panel's own padding above/below the projected content (measured: 24px, the
    /// library's spacing token). The defect left several hundred px of empty panel below the buttons, so
    /// anything near this bound is a content-sized panel and anything above it is the regression.
    /// </summary>
    private const int MaxPanelPaddingPx = 40;

    /// <summary>
    /// Measures the shadow-DOM panel (<c>&lt;dialog part="dialog"&gt;</c>) against the content projected
    /// into it: the light-DOM <c>[slot]</c> children (title, label, input, actions) are what the panel is
    /// supposed to wrap. Everything is computed from live boxes - no hard-coded panel height.
    /// </summary>
    private const string DialogPanelMetricsScript =
        """
        () => {
            const dialog = document.querySelector('fluent-dialog');
            const panel = dialog?.shadowRoot?.querySelector('dialog');
            const box = (el) => {
                const r = el.getBoundingClientRect();
                return { top: Math.round(r.top), bottom: Math.round(r.bottom), w: Math.round(r.width), h: Math.round(r.height) };
            };
            if (!panel) return JSON.stringify({ error: 'no shadow-dialog panel', viewport: { w: innerWidth, h: innerHeight } });

            const panelBox = box(panel);
            const slots = [...dialog.querySelectorAll('[slot]')]
                .filter(el => el.getBoundingClientRect().height > 0)
                .map(el => ({ slot: el.getAttribute('slot'), ...box(el) }));

            const contentTop = slots.length ? Math.min(...slots.map(s => s.top)) : panelBox.top;
            const contentBottom = slots.length ? Math.max(...slots.map(s => s.bottom)) : panelBox.bottom;

            return JSON.stringify({
                viewport: { w: innerWidth, h: innerHeight },
                panel: panelBox,
                slots: slots,
                contentTop: contentTop,
                contentBottom: contentBottom,
                contentHeight: contentBottom - contentTop,
                emptyAboveContent: contentTop - panelBox.top,
                emptyBelowContent: panelBox.bottom - contentBottom,
                panelCssHeight: getComputedStyle(panel).height,
                panelCssMaxHeight: getComputedStyle(panel).maxHeight
            });
        }
        """;
}
