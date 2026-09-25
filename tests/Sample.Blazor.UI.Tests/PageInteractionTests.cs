using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Blazor.UI.Tests.Infrastructure;

namespace Sample.Blazor.UI.Tests;

/// <summary>
/// Items 4 and 5 of the v5 migration validation: the interactive counter and the streamed weather
/// table still work after the migration.
/// </summary>
[TestClass]
public class PageInteractionTests : SamplePageTest
{
    [TestMethod]
    public async Task Counter_ClickingTheButtonIncrementsTheDisplayedCount()
    {
        await NavigateAsync(SampleDom.CounterRoute);

        var status = Page.Locator("p[role='status']");
        await Expect(status).ToHaveTextAsync("Current count: 0");

        var button = Page.GetByRole(AriaRole.Button, new() { Name = "Click me" });
        await Expect(button).ToBeVisibleAsync();

        await button.ClickAsync();
        await Expect(status).ToHaveTextAsync("Current count: 1");

        await button.ClickAsync();
        await button.ClickAsync();
        await Expect(status).ToHaveTextAsync("Current count: 3");

        Log($"counter after 3 clicks: '{await status.InnerTextAsync()}'");
        await AssertBaselineAsync("counter");
    }

    [TestMethod]
    public async Task Weather_TheTableStreamsFiveRows()
    {
        await NavigateAsync(SampleDom.WeatherRoute);

        await Expect(Page.Locator("h1")).ToHaveTextAsync("Weather Sample");

        // Streaming rendering: the table arrives after OnInitializedAsync finishes.
        var rows = Page.Locator("table.table tbody tr");
        await Expect(rows).ToHaveCountAsync(5);

        var summaries = await rows.AllInnerTextsAsync();
        Log($"weather rows: {summaries.Count} -> {string.Join(" || ", summaries.Select(r => r.Replace(Environment.NewLine, " ").Trim()))}");

        Assert.AreEqual(5, await rows.CountAsync(), "The forecast table must contain exactly 5 data rows.");
        Assert.AreEqual(4, await Page.Locator("table.table thead th").CountAsync(), "Expected 4 header columns.");

        await AssertBaselineAsync("weather");
    }
}
