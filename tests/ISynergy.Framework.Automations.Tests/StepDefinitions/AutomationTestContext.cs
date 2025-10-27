using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Tests.Fixtures;

namespace ISynergy.Framework.Automations.Tests.StepDefinitions;

/// <summary>
/// Shared context for automation test scenarios.
/// Allows state sharing between different step definition classes.
/// </summary>
public class AutomationTestContext
{
    public IAutomationService? AutomationService { get; set; }
    public Customer? Customer { get; set; }
    public Automation? Automation { get; set; }
}
