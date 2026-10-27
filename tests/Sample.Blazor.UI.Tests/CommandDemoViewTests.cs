using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// Regression guards for the command demo page's own chrome (as opposed to its dialog, which lives in
/// <see cref="DialogTests"/>).
/// </summary>
[TestClass]
public class CommandDemoViewTests : SamplePageTest
{
    private static readonly string[] ExpectedButtonLabels =
        ["Decrement", "Reset", "Open Sample Dialog", "Increment"];

    /// <summary>
    /// The four page buttons are direct children of <c>#article</c>, a column flex container, so a plain
    /// <c>&lt;button&gt;</c> is blockified and stretches to the full content width unless
    /// <c>align-self: flex-start</c> shrinks it back to its content. Measured before the fix: 989px in a
    /// 1024px content column; after: the buttons are only as wide as their labels (60-155px). This test
    /// fails if any of them stretches towards the column width again.
    /// </summary>
    [TestMethod]
    public async Task CommandDemoView_TheFourPageButtonsAreNotStretchedToFullWidth()
    {
        await NavigateAsync(SampleDom.CommandDemoRoute);

        var buttons = Page.Locator($"{SampleDom.ContentArticle} > button");
        await Expect(buttons).ToHaveCountAsync(ExpectedButtonLabels.Length);

        var measurements = await Page.Locator(SampleDom.ContentArticle).EvaluateAsync<string>(
            """
            (article) => {
                const box = (el) => {
                    const r = el.getBoundingClientRect();
                    return { w: Math.round(r.width), h: Math.round(r.height) };
                };
                return JSON.stringify({
                    viewportWidth: innerWidth,
                    article: box(article),
                    buttons: [...article.querySelectorAll(':scope > button')].map(b => ({
                        text: b.textContent.trim(),
                        ...box(b),
                        display: getComputedStyle(b).display,
                        alignSelf: getComputedStyle(b).alignSelf
                    }))
                });
            }
            """);

        Log($"command demo button measurements: {measurements}");

        var state = System.Text.Json.JsonDocument.Parse(measurements).RootElement;
        var articleWidth = state.GetProperty("article").GetProperty("w").GetInt32();
        var observed = state.GetProperty("buttons").EnumerateArray()
            .Select(b => (Text: b.GetProperty("text").GetString() ?? string.Empty,
                          Width: b.GetProperty("w").GetInt32(),
                          Height: b.GetProperty("h").GetInt32()))
            .ToList();

        CollectionAssert.AreEqual(
            ExpectedButtonLabels,
            observed.Select(b => b.Text).ToArray(),
            $"{SampleDom.CommandDemoRoute} must render exactly the four expected buttons in order. " +
            $"Observed: {measurements}");

        Assert.IsTrue(articleWidth > 0, $"The content column has no width. Measurements: {measurements}");

        // A stretched button is ~ the whole column; a content-sized one is a small fraction of it. The
        // bound is relative to the live column width, so it does not hard-code any button size.
        var maxWidth = articleWidth / 2.0;
        foreach (var button in observed)
        {
            Assert.IsTrue(
                button.Width > 0 && button.Height > 0,
                $"The '{button.Text}' button has no rendered box. Measurements: {measurements}");

            Assert.IsTrue(
                button.Width <= maxWidth,
                $"The '{button.Text}' button is stretched to {button.Width}px of the {articleWidth}px content " +
                $"column (limit {maxWidth:F0}px): it lost its content sizing - the flex column blockifies it " +
                $"again (this measured ~989px before the fix). Measurements: {measurements}");
        }

        Log($"all {observed.Count} buttons are content-sized: " +
            $"{string.Join(", ", observed.Select(b => $"{b.Text}={b.Width}px"))} in a {articleWidth}px column.");
    }
}
