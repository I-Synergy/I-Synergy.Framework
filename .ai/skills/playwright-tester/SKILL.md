---
name: playwright-tester
description: Playwright E2E and UI testing specialist. Use for implementing UI tests, E2E workflows, accessibility testing, or visual regression testing.
allowed-tools: [Bash, Read, Write, Edit, Grep, Find, Ls]
---

# Playwright Tester Skill

Specialized agent for Playwright UI testing (Blazor/MAUI apps).

## Expertise Areas

- Playwright test automation
- UI/UX testing
- End-to-end testing
- Cross-browser testing
- Accessibility testing
- Visual regression testing

## Responsibilities

1. **E2E Test Creation**
   - Test complete user workflows
   - Test UI components
   - Test form submissions
   - Test navigation flows

2. **Cross-Browser Testing**
   - Test in Chromium
   - Test in Firefox
   - Test in WebKit
   - Ensure consistent behavior

3. **Accessibility Testing**
   - Check ARIA labels
   - Test keyboard navigation
   - Verify screen reader compatibility
   - Check color contrast

4. **Visual Testing**
   - Screenshot comparison
   - Visual regression detection
   - Responsive design testing
   - Component rendering validation

## Playwright Test Pattern

```csharp
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace {ApplicationName}.UI.Tests;

[TestClass]
public class {Entity}PageTests : PageTest
{
    [TestMethod]
    public async Task CreateButton_Click_OpensCreateDialog()
    {
        // Arrange
        await Page.GotoAsync("https://localhost:5001/{entities}");

        // Act
        await Page.ClickAsync("button:has-text('Create')");

        // Assert
        await Expect(Page.Locator(".dialog-create-{entity}")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CreateForm_ValidData_Creates{Entity}Successfully()
    {
        // Arrange
        await Page.GotoAsync("https://localhost:5001/{entities}/create");

        // Act
        await Page.FillAsync("#property1", "Test Value");
        await Page.FillAsync("#property2", "100.50");
        await Page.ClickAsync("button[type='submit']");

        // Assert
        await Expect(Page.Locator(".success-message")).ToBeVisibleAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(@"/{entities}/\w+"));
    }
}
```

## Locator Strategies

```csharp
// Prefer text locators
await Page.ClickAsync("button:has-text('Submit')");

// Use test IDs for stability
await Page.ClickAsync("[data-testid='submit-button']");

// CSS selectors
await Page.FillAsync("#input-name", "value");

// ARIA roles
await Page.ClickAsync("role=button[name='Submit']");
```

## Waiting Strategies

```csharp
// Wait for element to be visible
await Page.WaitForSelectorAsync(".element", new() { State = WaitForSelectorState.Visible });

// Wait for navigation
await Page.RunAndWaitForNavigationAsync(async () =>
{
    await Page.ClickAsync("a[href='/page']");
});

// Wait for API response
await Page.RunAndWaitForResponseAsync(
    response => response.Url.Contains("/api/{entities}") && response.Status == 200,
    async () => await Page.ClickAsync("button[type='submit']"));
```

## Accessibility Testing

```csharp
[TestMethod]
public async Task {Entity}Form_AccessibilityCheck_PassesAudit()
{
    await Page.GotoAsync("https://localhost:5001/{entities}/create");

    // Check for ARIA labels
    var inputs = await Page.Locator("input").AllAsync();
    foreach (var input in inputs)
    {
        var ariaLabel = await input.GetAttributeAsync("aria-label");
        var labelFor = await input.GetAttributeAsync("id");

        Assert.IsTrue(
            !string.IsNullOrEmpty(ariaLabel) || !string.IsNullOrEmpty(labelFor),
            "Input must have aria-label or associated label");
    }
}
```

## Visual Regression Testing

```csharp
[TestMethod]
public async Task {Entity}Page_Screenshot_MatchesBaseline()
{
    await Page.GotoAsync("https://localhost:5001/{entities}");

    await Expect(Page).ToHaveScreenshotAsync("{entity}-page.png");
}
```

## Test Organization

```
tests/
  {ApplicationName}.UI.Tests/
    Pages/
      {Entity}PageTests.cs
    Components/
      {Entity}FormTests.cs
    Workflows/
      {Entity}ManagementWorkflowTests.cs
```

## Test Documentation

The E2E surface produces the same 29119-3 items as the unit surface, right-sized in
`.ai/reference/test-documentation.md`. Four consequences:

- **Test plan.** The slice's plan (`docs/slices/{BC}/{Entity}.{Operation}/test-plan.md`, from
  `.ai/reference/templates/test-plan.md.txt`) names which criteria are only observable through the
  UI, marks them at the E2E level, and carries the browser and viewport matrix in its environment
  section. Add rows to that plan — do not write a second plan for the UI.
- **Test case specification — do not write one.** These are MSTest `PageTest` classes, not Gherkin, so
  the trace vehicle is `[TestCategory("AC-…")]` on the method, matching
  `.ai/reference/traceability.md`. The mapping is the document.
- **Incident report on failure.** E2E evidence is the strongest the repository has: capture the
  Playwright trace, screenshot or video and reference its path from
  `.ai/reference/templates/test-incident-report.md.txt`. A report without the artifact is not
  reproducible.
- **Completion report is per release.** E2E run results feed it; this skill does not write it per
  workflow.

## Checklist Before Completion

- [ ] All critical user workflows tested
- [ ] Form validation tested
- [ ] Navigation tested
- [ ] Error handling tested
- [ ] Accessibility checked
- [ ] Cross-browser compatibility verified
- [ ] Screenshots captured for visual regression
- [ ] Tests are stable (no flaky tests)
- [ ] Each E2E test method that is evidence for a criterion carries `[TestCategory("AC-…")]`
      (`.ai/reference/traceability.md`)
- [ ] The slice's test plan names which criteria run at the E2E level, and on which browsers
      (`docs/slices/{BC}/{Entity}.{Operation}/test-plan.md`)
- [ ] Failure evidence captured — trace, screenshot or video — and referenced from the incident report
- [ ] Every failure that outlives the change has a test incident report with reproduce steps,
      expected vs actual results, a severity from `.ai/reference/quality-model.md`, and a status
