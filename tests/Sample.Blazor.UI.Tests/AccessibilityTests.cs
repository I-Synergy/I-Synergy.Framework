using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// Item 10 of the v5 migration validation: basic accessibility checks on the home page and on the open
/// dialog — accessible names for interactive elements (including inputs inside the web components'
/// shadow DOM), heading order, and a computed colour contrast sweep.
/// <para>
/// These are hand rolled checks, not an axe-core audit: everything is computed from the live DOM so the
/// suite has no external dependency and no remote script to load. Interactive elements are found with
/// Playwright locators (which pierce open shadow roots) rather than <c>document.querySelectorAll</c>
/// (which does not), because the dialog's text input only exists inside <c>fluent-text-input</c>'s
/// shadow DOM.
/// </para>
/// </summary>
[TestClass]
public class AccessibilityTests : SamplePageTest
{
    /// <summary>WCAG AA floor for large text / non-text UI.</summary>
    private const double MinimumContrast = 3.0;

    /// <summary>WCAG AA floor for body text; reported as a warning, not enforced.</summary>
    private const double BodyTextContrast = 4.5;

    [TestMethod]
    public async Task Home_InteractiveElements_HaveAccessibleNames()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await AssertInteractiveNamesAsync("home");
    }

    [TestMethod]
    public async Task Home_HeadingsAreInOrder()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await AssertHeadingOrderAsync("home");
    }

    [TestMethod]
    public async Task Home_HasNoObviousContrastFailures()
    {
        await NavigateAsync(SampleDom.HomeRoute);
        await AssertNoContrastFailuresAsync("home");
    }

    [TestMethod]
    public async Task Dialog_InputsHaveAccessibleNames()
    {
        await NavigateAsync(SampleDom.CommandDemoRoute);

        var open = Page.GetByRole(AriaRole.Button, new() { Name = "Open Sample Dialog" });
        await open.ClickAsync();
        await Expect(Page.Locator(SampleDom.Dialog)).ToHaveCountAsync(1);

        await AssertInteractiveNamesAsync("dialog");

        // The browser's own accessibility tree of the dialog: proves the shadow-DOM input is exposed
        // as a named textbox (not an unnamed control) and that the dialog exposes a dialog role.
        var snapshot = await Page.Locator(SampleDom.Dialog).AriaSnapshotAsync();
        Log($"{Environment.NewLine}[evidence] dialog accessibility tree:{Environment.NewLine}{snapshot}");
        StringAssert.Contains(snapshot, "textbox \"Description\"",
            "The dialog's text input is not exposed as a named textbox.");

        await AssertNoContrastFailuresAsync("dialog");
    }

    /// <summary>
    /// Item 10 — heading order on the command demo page. The page renders exactly one <c>&lt;h1&gt;</c>
    /// ("Command Binding Demo"), like every other page in the sample, so its document outline starts at
    /// level 1. This is a regression guard: a page that begins with an <c>&lt;h3&gt;</c> and no preceding
    /// <c>&lt;h1&gt;</c> starts its outline at level 3, which is an accessibility defect, so this test fails
    /// if the page loses its single <c>&lt;h1&gt;</c> or regresses to that shape.
    /// </summary>
    [TestMethod]
    public async Task CommandDemoView_HeadingStructureShouldStartWithAnH1()
    {
        await NavigateAsync(SampleDom.CommandDemoRoute);
        await AssertHeadingOrderAsync("commanddemoview");
    }

    private async Task AssertInteractiveNamesAsync(string label)
    {
        var elements = Page.Locator("a, button, input, select, textarea, fluent-button");
        var total = await elements.CountAsync();

        var unnamed = new List<string>();
        var checkedCount = 0;

        for (var i = 0; i < total; i++)
        {
            var element = elements.Nth(i);

            if (!await element.IsVisibleAsync())
                continue;

            var json = await element.EvaluateAsync<string>(NameProbeScript);
            using var document = System.Text.Json.JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.GetProperty("skip").GetBoolean())
                continue;

            checkedCount++;
            var name = root.GetProperty("name").GetString() ?? string.Empty;
            var descriptor = root.GetProperty("descriptor").GetString() ?? string.Empty;

            if (name.Length == 0)
                unnamed.Add(descriptor);
            else
                Console.WriteLine($"[evidence] {label} named element: {descriptor} -> '{Truncate(name)}'");
        }

        Assert.AreEqual(
            0,
            unnamed.Count,
            $"{label}: interactive element(s) without an accessible name:{Environment.NewLine}{string.Join(Environment.NewLine, unnamed)}");

        Assert.AreNotEqual(0, checkedCount, $"{label}: no visible interactive element was found to check.");
        Log($"{label}: {checkedCount} visible interactive element(s) checked, all have accessible names.");
    }

    private async Task AssertHeadingOrderAsync(string label)
    {
        var json = await Page.EvaluateAsync<string>(HeadingScript);
        Log($"{label} headings: {json}");

        using var document = System.Text.Json.JsonDocument.Parse(json);
        var headings = document.RootElement.EnumerateArray()
            .Select(h => (Level: h.GetProperty("level").GetInt32(), Text: h.GetProperty("text").GetString() ?? string.Empty))
            .ToList();

        if (headings.Count == 0)
            return;

        Assert.AreEqual(1, headings.Count(h => h.Level == 1), $"{label}: expected exactly one <h1>.");
        Assert.AreEqual(1, headings[0].Level, $"{label}: the first heading must be an <h1> but was <h{headings[0].Level}>.");

        for (var i = 1; i < headings.Count; i++)
        {
            Assert.IsTrue(
                headings[i].Level <= headings[i - 1].Level + 1,
                $"{label}: heading level jumps from <h{headings[i - 1].Level}> '{headings[i - 1].Text}' " +
                $"to <h{headings[i].Level}> '{headings[i].Text}'.");
        }
    }

    private async Task AssertNoContrastFailuresAsync(string label)
    {
        var json = await Page.EvaluateAsync<string>(ContrastScript);
        using var document = System.Text.Json.JsonDocument.Parse(json);
        var findings = document.RootElement.EnumerateArray()
            .Select(e => new
            {
                Tag = e.GetProperty("tag").GetString(),
                Text = e.GetProperty("text").GetString(),
                Ratio = Math.Round(e.GetProperty("ratio").GetDouble(), 2),
                Foreground = e.GetProperty("foreground").GetString(),
                Background = e.GetProperty("background").GetString(),
                FontSize = e.GetProperty("fontSize").GetDouble()
            })
            .ToList();

        var lowest = findings.OrderBy(f => f.Ratio).FirstOrDefault();
        Log($"{label} contrast: {findings.Count} text element(s) measured; " +
            $"lowest ratio {(lowest is null ? "n/a" : $"{lowest.Ratio} ({lowest.Tag} '{Truncate(lowest.Text)}')")}");

        var failures = findings.Where(f => f.Ratio < MinimumContrast).ToList();
        var warnings = findings.Where(f => f.Ratio is >= MinimumContrast and < BodyTextContrast).ToList();

        foreach (var warning in warnings)
            Console.WriteLine($"[evidence] {label} contrast warning (< {BodyTextContrast}): " +
                              $"{warning.Tag} '{Truncate(warning.Text)}' ratio={warning.Ratio} " +
                              $"color={warning.Foreground} on {warning.Background} size={warning.FontSize}px");

        Assert.AreEqual(
            0,
            failures.Count,
            $"{label}: text below the {MinimumContrast}:1 contrast floor:{Environment.NewLine}" +
            string.Join(Environment.NewLine, failures.Select(f =>
                $"{f.Tag} '{Truncate(f.Text)}' ratio={f.Ratio} color={f.Foreground} on {f.Background} size={f.FontSize}px")));
    }

    private static string Truncate(string? value) =>
        value is null ? string.Empty : value.Length <= 40 ? value : value[..40] + "...";

    /// <summary>
    /// Runs against a single element (Playwright resolves it, shadow-DOM included) and returns the
    /// accessible name plus a descriptor. Elements inside someone else's shadow root are skipped unless
    /// they are inputs, because their host element is the accessibility-relevant one.
    /// </summary>
    private const string NameProbeScript =
        """
        (el) => {
            const name = (e) => {
                const aria = e.getAttribute('aria-label');
                if (aria && aria.trim()) return aria.trim();

                const labelledBy = e.getAttribute('aria-labelledby');
                if (labelledBy) {
                    const root = e.getRootNode();
                    const text = labelledBy.split(/\s+/)
                        .map(id => (root.getElementById ? root.getElementById(id)?.textContent : '') || document.getElementById(id)?.textContent || '')
                        .join(' ').trim();
                    if (text) return text;
                }

                if (e.labels && e.labels.length > 0) return e.labels[0].textContent.trim();
                if (e.closest && e.closest('label')) return e.closest('label').textContent.trim();
                if (e.tagName === 'INPUT' && ['submit', 'button', 'reset'].includes(e.type)) return (e.value || '').trim();
                if (e.title && e.title.trim()) return e.title.trim();
                return (e.textContent || '').trim();
            };

            const inShadow = el.getRootNode() !== document;
            const skip = inShadow && el.tagName !== 'INPUT';

            return JSON.stringify({
                skip: skip,
                inShadow: inShadow,
                name: skip ? '' : name(el),
                descriptor: el.tagName.toLowerCase() +
                    (el.id ? '#' + el.id : '') +
                    (el.tagName === 'INPUT' ? `[type=${el.type}]` : '') +
                    `[text='${(el.textContent || '').trim().slice(0, 30)}']` +
                    (inShadow ? '[shadow]' : '')
            });
        }
        """;

    private const string HeadingScript =
        """
        () => {
            const visible = (el) => { const r = el.getBoundingClientRect(); return r.width > 0 && r.height > 0; };
            return JSON.stringify([...document.querySelectorAll('h1,h2,h3,h4,h5,h6')]
                .filter(visible)
                .map(h => ({ level: parseInt(h.tagName.substring(1), 10), text: h.textContent.trim().slice(0, 60) })));
        }
        """;

    private const string ContrastScript =
        """
        () => {
            const parse = (value) => {
                const match = value && value.match(/rgba?\(([^)]+)\)/);
                if (!match) return null;
                const parts = match[1].split(',').map(p => parseFloat(p.trim()));
                return { r: parts[0], g: parts[1], b: parts[2], a: parts.length > 3 ? parts[3] : 1 };
            };
            const luminance = (c) => {
                const channel = (v) => { v /= 255; return v <= 0.03928 ? v / 12.92 : Math.pow((v + 0.055) / 1.055, 2.4); };
                return 0.2126 * channel(c.r) + 0.7152 * channel(c.g) + 0.0722 * channel(c.b);
            };
            const background = (el) => {
                let node = el;
                while (node) {
                    const c = parse(getComputedStyle(node).backgroundColor);
                    if (c && c.a > 0.05) return c;
                    node = node.parentElement;
                }
                return { r: 255, g: 255, b: 255, a: 1 };
            };

            const results = [];
            for (const el of document.querySelectorAll('body *')) {
                const own = [...el.childNodes]
                    .filter(n => n.nodeType === 3)
                    .map(n => n.textContent.trim())
                    .join(' ')
                    .trim();
                if (!own) continue;

                const rect = el.getBoundingClientRect();
                if (rect.width < 1 || rect.height < 1) continue;

                const style = getComputedStyle(el);
                if (style.visibility === 'hidden' || style.display === 'none' || parseFloat(style.opacity) < 0.5) continue;

                const fg = parse(style.color);
                if (!fg || fg.a < 0.5) continue;

                const bg = background(el);
                const l1 = luminance(fg);
                const l2 = luminance(bg);
                const ratio = (Math.max(l1, l2) + 0.05) / (Math.min(l1, l2) + 0.05);

                results.push({
                    tag: el.tagName.toLowerCase() + (el.className && typeof el.className === 'string' ? '.' + el.className.trim().split(/\s+/)[0] : ''),
                    text: own.slice(0, 60),
                    ratio: ratio,
                    foreground: style.color,
                    background: `rgb(${Math.round(bg.r)}, ${Math.round(bg.g)}, ${Math.round(bg.b)})`,
                    fontSize: parseFloat(style.fontSize)
                });
            }
            return JSON.stringify(results);
        }
        """;
}
