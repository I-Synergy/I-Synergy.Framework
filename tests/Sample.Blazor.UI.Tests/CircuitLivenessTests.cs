using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// The verification blind spot that let a circuit-killing defect through green: every health assertion
/// in this suite ran on the dialog's <b>open</b> path, so nothing observed the state after a dialog
/// closed. "The dialog closed" is not the same as "the application survived" — a dead Blazor Server
/// circuit keeps rendering the DOM of its last render and silently ignores every click, so a closed
/// dialog over a dead circuit is indistinguishable from a healthy one until something tries to
/// interact with the page again.
/// <para>
/// Every test here therefore closes a dialog and then proves the circuit still processes input by
/// clicking "Increment" and asserting the counter readout actually incremented. That positive liveness
/// step is the assertion that would have caught the reported defect.
/// </para>
/// </summary>
[TestClass]
public class CircuitLivenessTests : SamplePageTest
{
    /// <summary>How long the re-render is given; a dead circuit never produces one, so this expires.</summary>
    private const int LivenessTimeoutMs = 10_000;

    private const string IncrementButtonName = "Increment";

    [TestMethod]
    public async Task Dialog_ClosedWithCancel_LeavesTheCircuitInteractive()
    {
        await OpenDialogAsync();
        await CloseDialogAsync(ClosePath.Cancel);
        await AssertCounterIncrementsAsync("after closing the dialog with Cancel");
    }

    [TestMethod]
    public async Task Dialog_ClosedWithSubmit_LeavesTheCircuitInteractive()
    {
        await OpenDialogAsync();
        await CloseDialogAsync(ClosePath.Submit);
        await AssertCounterIncrementsAsync("after closing the dialog with Add");
    }

    /// <summary>
    /// The reported defect froze the page after the <b>first</b> close, so a single open/close cycle is
    /// not sufficient evidence: this cycles three times, alternating the close path (Cancel, Add,
    /// Cancel), and re-proves the circuit is live after every single close.
    /// </summary>
    [TestMethod]
    public async Task Dialog_RepeatedOpenAndCloseCycles_LeaveTheCircuitInteractive()
    {
        await OpenDialogAsync();

        var closePaths = new[] { ClosePath.Cancel, ClosePath.Submit, ClosePath.Cancel };

        for (var cycle = 1; cycle <= closePaths.Length; cycle++)
        {
            if (cycle > 1)
                await OpenDialogAgainAsync(cycle);

            await CloseDialogAsync(closePaths[cycle - 1]);
            await AssertCounterIncrementsAsync($"after cycle {cycle} (closed with {closePaths[cycle - 1]})");
        }
    }

    /// <summary>The two ways this sample's dialog can be closed from its own footer.</summary>
    private enum ClosePath
    {
        Cancel,
        Submit
    }

    private async Task OpenDialogAsync()
    {
        await NavigateAsync(SampleDom.CommandDemoRoute);
        await ClickOpenDialogAsync("initial open");
    }

    private async Task OpenDialogAgainAsync(int cycle) => await ClickOpenDialogAsync($"cycle {cycle} open");

    private async Task ClickOpenDialogAsync(string context)
    {
        var open = Page.GetByRole(AriaRole.Button, new() { Name = "Open Sample Dialog" });
        await Expect(open).ToBeVisibleAsync();
        await open.ClickAsync();

        await Expect(Page.Locator(SampleDom.Dialog)).ToHaveCountAsync(
            1,
            new LocatorAssertionsToHaveCountOptions { Timeout = LivenessTimeoutMs });

        Log($"{context}: dialog rendered (count=1).");
    }

    private async Task CloseDialogAsync(ClosePath path)
    {
        var label = path == ClosePath.Cancel ? "Cancel" : "Add";
        var button = Page.Locator(SampleDom.Dialog)
            .Locator("fluent-button")
            .Filter(new LocatorFilterOptions { HasTextString = label });

        Assert.AreEqual(1, await button.CountAsync(),
            $"The dialog must render exactly one '{label}' button to close it through {path}.");

        await button.ClickAsync();

        // Necessary but not sufficient: the dialog disappearing does not prove the circuit survived it.
        await Expect(Page.Locator(SampleDom.Dialog)).ToHaveCountAsync(0);
        Log($"dialog closed via '{label}' ({await DescribeDialogStateAsync()}).");
    }

    /// <summary>
    /// The liveness proof: the counter must actually change after the click. A dead circuit would leave
    /// the (stale) DOM in place and never apply the increment, which is exactly the failure mode the
    /// suite previously missed.
    /// </summary>
    private async Task AssertCounterIncrementsAsync(string context)
    {
        var counter = Page.Locator(SampleDom.CommandDemoCounter);
        Assert.AreEqual(1, await counter.CountAsync(),
            $"{context}: expected exactly one 'Current count' readout on {SampleDom.CommandDemoRoute}.");

        var before = await ReadCounterAsync(counter, context);
        var increment = Page.GetByRole(AriaRole.Button, new() { Name = IncrementButtonName });
        await Expect(increment).ToBeEnabledAsync();

        await increment.ClickAsync();

        try
        {
            await Expect(counter).ToHaveTextAsync(
                $"Current count: {before + 1}",
                new LocatorAssertionsToHaveTextOptions { Timeout = LivenessTimeoutMs });
        }
        catch (Exception)
        {
            await TryLogLivenessStateAsync(context, counter, before);
            throw;
        }

        var after = await ReadCounterAsync(counter, context);
        Log($"{context}: counter incremented {before} -> {after}; the circuit processed the click.");
    }

    private async Task<int> ReadCounterAsync(ILocator counter, string context)
    {
        var text = (await counter.InnerTextAsync()).Trim();
        var match = Regex.Match(text, @"Current count:\s*(?<value>-?\d+)");
        Assert.IsTrue(match.Success, $"{context}: could not parse a counter value out of '{text}'.");

        return int.Parse(match.Groups["value"].Value, CultureInfo.InvariantCulture);
    }

    /// <summary>Records what the page looks like after a failed liveness check without replacing the failure.</summary>
    private async Task TryLogLivenessStateAsync(string context, ILocator counter, int before)
    {
        try
        {
            Log($"{context}: FAILED liveness check - clicking '{IncrementButtonName}' did not change the counter. " +
                $"observed='{await counter.InnerTextAsync()}' expected='Current count: {before + 1}' " +
                $"blazorErrorUiVisible={await Page.Locator(SampleDom.BlazorErrorUi).IsVisibleAsync()} " +
                $"{await DescribeDialogStateAsync()}");
        }
        catch
        {
            // Diagnostics must never mask the assertion they were collected for.
        }

        ReportDiagnostics(context);

        // The full console capture is the main clue for *why* the click was ignored (a disconnect, a
        // timeout, a server-side exception); it is deliberately not truncated.
        foreach (var line in ConsoleCapture)
            Log($"{context}: console {line}");
    }

    private async Task<string> DescribeDialogStateAsync()
    {
        return await Page.Locator(SampleDom.DialogProvider).EvaluateAsync<string>(
            """
            (provider) => {
                const dialogs = [...provider.querySelectorAll('fluent-dialog')];
                return JSON.stringify({
                    dialogHostCount: dialogs.length,
                    providerChildren: [...provider.children].map(c => c.tagName.toLowerCase()),
                    shadowDialogsOpen: dialogs.map(d => d.shadowRoot?.querySelector('dialog')?.open ?? 'no shadow root'),
                    overlayVisible: (() => {
                        const overlay = provider.querySelector('fluent-overlay');
                        const dialog = overlay?.shadowRoot?.querySelector('dialog');
                        return dialog?.open ?? overlay?.hasAttribute('visible') ?? null;
                    })()
                });
            }
            """);
    }
}
